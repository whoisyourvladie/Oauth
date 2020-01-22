using Microsoft.Owin.Security.OAuth;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Oauth;
using SaaS.Data.Entities.View;
using SaaS.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SaaS.Api.Oauth.Providers
{
    internal class SaaSScopeWorkerFactory
    {
        private readonly static StringComparer _comparer = StringComparer.InvariantCultureIgnoreCase;

        internal static SaaSScopeBase Create(string[] scope)
        {
            if (!object.Equals(scope, null))
            {
                if (scope.Contains("editor", _comparer))
                    return new EditorSaaSScopeHelper();

                if (scope.Contains("webeditor", _comparer))
                    return new WebEditorSaaSScopeWorker();
            }

            return new DefaultSaaSScopeHelper();
        }

        internal static SaaSScopeBase Create(string[] scope, IAuthRepository auth = null, IAuthProductRepository authProduct = null)
        {
            var worker = Create(scope);

            worker.Auth = auth;
            worker.AuthProduct = authProduct;

            return worker;
        }

        internal static SaaSScopeBase Create(string scope, IAuthRepository auth = null, IAuthProductRepository authProduct = null)
        {
            return Create(StringToScope(scope), auth, authProduct);
        }

        internal static SaaSScopeBase Create(string[] scope, BaseValidatingTicketContext<OAuthAuthorizationServerOptions> context, AuthRepository auth, AuthProductRepository authProduct)
        {
            var worker = Create(scope, auth, authProduct);

            worker.Context = context;

            return worker;
        }

        internal static string ScopeToString(string[] scope)
        {
            return object.Equals(scope, null) || scope.Length == 0 ? null : string.Join(",", scope);
        }
        internal static string[] StringToScope(string scope)
        {
            if (string.IsNullOrEmpty(scope))
                return null;

            return scope.Split(',');
        }
    }

    internal abstract class SaaSScopeBase
    {
        internal BaseValidatingTicketContext<OAuthAuthorizationServerOptions> Context;
        internal IAuthRepository Auth;
        internal IAuthProductRepository AuthProduct;

        internal virtual async Task<bool> ValidateDeviceAsync(Account user)
        {
            return await Task.FromResult(true);
        }

        internal virtual bool ValidateSessionTokenAsync(Account user, IEnumerable<SessionToken> sessions, SessionToken session, ViewAccountProduct product)
        {
            return true;
        }

        internal virtual async Task<bool> ValidateAccountSystemAsync(Account user, SessionToken session, ViewAccountProduct product)
        {
            return await Task.FromResult(true);
        }

        internal virtual async Task SessionTokenInsertAsync(SessionToken token, Guid? oldId)
        {
            await Auth.SessionTokenInsertAsync(token, oldId, false, false);
        }

        internal virtual void FilterProducts(IList<ViewAccountProduct> products, SessionToken session) { }

        internal virtual async Task GrantClaims(ClaimsIdentity identity)
        {
            var sessionToken = Context.OwinContext.Get<SessionToken>("sessionToken");

            if (sessionToken.AccountProductId.HasValue)
            {
                var products = await AuthProduct.AccountProductsGetAsync(sessionToken.AccountId, sessionToken.SystemId);
                var product = products.FirstOrDefault(e => e.AccountProductId == sessionToken.AccountProductId.Value);

                if (object.Equals(product, null) || product.IsDisabled)
                    Context.Response.Headers.Add("Product-IsDisabled", new[] { bool.TrueString });

                if (object.Equals(product, null) || product.IsExpired)
                    Context.Response.Headers.Add("Product-IsExpired", new[] { bool.TrueString });

                if (!object.Equals(product, null) && !product.IsDisabled)
                    UserManagerHelper.GetAllowedModules(products, sessionToken.AccountProductId.Value, identity);
            }
        }
    }
    internal sealed class DefaultSaaSScopeHelper : SaaSScopeBase
    {
        internal override Task GrantClaims(ClaimsIdentity identity)
        {
            return Task.Run(() =>
            {

            });
        }
    }
    internal sealed class EditorSaaSScopeHelper : SaaSScopeBase
    {

        private readonly static StringComparison _comparison = StringComparison.InvariantCultureIgnoreCase;

        internal override async Task<bool> ValidateDeviceAsync(Account user)
        {
            SystemSignInData data = Context.OwinContext.Get<SystemSignInData>("systemSignInData") ?? new SystemSignInData { };
            if ((object.Equals(data, null)) || (data.MachineKey == Guid.Empty &&
                                                string.IsNullOrEmpty(data.MotherboardKey) &&
                                                string.IsNullOrEmpty(data.PhysicalMac)))
            {
                Context.SetError("invalid_grant", "System is null or empty.");
                return false;
            }

            var system = await Auth.SystemInsertAsync(new Data.Entities.Oauth.OauthSystem
            {
                MachineKey = data.MachineKey,
                MotherboardKey = data.MotherboardKey,
                PhysicalMac = data.PhysicalMac,
                IsAutogeneratedMachineKey = data.IsAutogeneratedMachineKey,
                PcName = data.PcName
            });

            Context.OwinContext.Set("systemId", system.Id);

            return true;
        }

        internal override bool ValidateSessionTokenAsync(Account user, IEnumerable<SessionToken> sessions, SessionToken session, ViewAccountProduct product)
        {
            var accountProductSessions = sessions.Where(e => (e.AccountProductId.HasValue && e.AccountProductId.Value == product.AccountProductId && e.SystemId.HasValue && e.SystemId.Value != session.SystemId.Value) ||
                                                             (e.Id == session.Id));
            var limit = 3;

#if PdfSam
            limit = 2;
#endif

#if LuluSoft

            if (product.IsFree)
                limit = 10;

#endif

            //if user is logged into more than {{limit}} devices
            return accountProductSessions.Count() <= limit;
        }

        internal override async Task<bool> ValidateAccountSystemAsync(Account account, SessionToken session, ViewAccountProduct product)
        {
            if (!product.IsPPC)
                return await Task.FromResult(true);

            var accountSystems = await Auth.AccountSystemsGetAsync(new AccountProductPair(account.Id, product.AccountProductId));
            var accountSystemsWithoutCurrent = accountSystems.Where(e => e.SystemId != session.SystemId.Value);

            var limit = 1;

            //if user's license associated more than {{limit}} devices
            return await Task.FromResult(accountSystemsWithoutCurrent.Count() < limit);
        }

        internal override async Task SessionTokenInsertAsync(SessionToken token, Guid? oldId)
        {
            await Auth.SessionTokenInsertAsync(token, oldId, false, false);
        }

        internal override void FilterProducts(IList<ViewAccountProduct> products, SessionToken session)
        {
            var clientVersion = session.GetClientVersion();
            if (object.Equals(clientVersion, null))
                return;

            for (int index = 0; index < products.Count; index++)
            {
                var product = products[index];
                if (!product.IsPPC) continue;

                var productVersion = product.GetProductVersion();
                if (object.Equals(productVersion, null))
                    continue;

                if (clientVersion.Major != productVersion.Major)
                    products.RemoveAt(index--);
            }

            if ("desktop".Equals(session.ClientName, _comparison))
            {
                for (int index = 0; index < products.Count; index++)
                {
                    var product = products[index];

#if LuluSoft
                    if ("soda-business-trial-2-week".Equals(product.ProductUnitName, _comparison) ||
                        "soda-business-trial-monthly".Equals(product.ProductUnitName, _comparison) ||
                        "soda-business-trial-3-month".Equals(product.ProductUnitName, _comparison) ||

                        "soda-business-monthly".Equals(product.ProductUnitName, _comparison) ||
                        "soda-business-yearly".Equals(product.ProductUnitName, _comparison) ||
                        "soda-business-2-year".Equals(product.ProductUnitName, _comparison))
                    {
                        var ocrTess = product.Modules.FirstOrDefault(module => "ocr-tess".Equals(module.Module, _comparison));
                        if (!object.Equals(ocrTess, null))
                            ocrTess.Module = "ocr";
                    }
#endif

#if PdfForge
                    if ("architect-free".Equals(product.ProductUnitName, _comparison) ||
                        "architect-esign-yearly".Equals(product.ProductUnitName, _comparison) ||
                        "architect-esign-10-pack".Equals(product.ProductUnitName, _comparison) ||
                        "architect-esign-trial-2-week".Equals(product.ProductUnitName, _comparison))
                    { }
                    else
                        products.RemoveAt(index--);
#endif
                    }
                }

#if PdfForge
            //PA-2746
            var hasOCR = products.Any(product =>
                ("architect-ocr-yearly".Equals(product.ProductUnitName, _comparison)
                || "architect-ocr-standard-yearly".Equals(product.ProductUnitName, _comparison))
                && !product.IsDisabled
                && !product.IsExpired
                && !product.IsTrial);

            if (hasOCR)
            {
                foreach (var proProd in products.Where(product =>
                        ("architect-pro-ppc".Equals(product.ProductUnitName, _comparison)
                        || "architect-pro-yearly".Equals(product.ProductUnitName, _comparison))
                        && !product.IsDisabled
                        && !product.IsExpired
                        && !product.IsTrial))
                {
                    proProd.IsUpgradable = false;
                }
            }
#endif
        }
    }
        internal sealed class WebEditorSaaSScopeWorker : SaaSScopeBase
    {
        internal override async Task SessionTokenInsertAsync(SessionToken token, Guid? oldId)
        {
            await Auth.SessionTokenInsertAsync(token, oldId, true, false);
        }

        internal override void FilterProducts(IList<ViewAccountProduct> products, SessionToken session)
        {
            for (int index = 0; index < products.Count; index++)
            {
                if (products[index].IsPPC)
                    products.RemoveAt(index--);
            }
        }
    }
}