using Microsoft.Owin.Security;
using SaaS.Api.Models.Products;
using SaaS.Api.Oauth.Providers;
using SaaS.Data.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace SaaS.Api.Oauth
{
    internal static class UserManagerHelper
    {
        private static readonly StringComparison _stringComparison = StringComparison.InvariantCultureIgnoreCase;

        private static IEnumerable<ViewAccountProduct> GetFilter(IEnumerable<ViewAccountProduct> products, Guid accountProductId)
        {
            return products.Where(e => !e.IsDisabled && (e.AccountProductId == accountProductId || e.IsMinor));
        }

        internal static IEnumerable<Guid> GetActiveProducts(IEnumerable<ViewAccountProduct> products, IEnumerable<AccountProductModuleModel> allowedModules, Guid accountProductId)
        {
            var activeProducts = new List<ViewAccountProduct>();

            var filter = GetFilter(products, accountProductId);
            var mainProduct = filter.FirstOrDefault(e => e.AccountProductId == accountProductId);
            if (!object.Equals(mainProduct, null))
            {
                activeProducts.Add(mainProduct);

                var minorProducts = filter.Where(e => e.AccountProductId != accountProductId).ToList();

                foreach (var module in allowedModules)
                {
                    if ("e-sign".Equals(module.Module, _stringComparison))
                    {
                        if (!mainProduct.AllowedEsignCount.HasValue)
                            continue; //ignore

                        var minorProduct = minorProducts.FirstOrDefault(e => !e.AllowedEsignCount.HasValue);
                        if (!object.Equals(minorProduct, null))
                        {
                            activeProducts.Add(minorProduct);
                            continue;
                        }

                        activeProducts.AddRange(minorProducts);

                        continue;
                    }

                    var mainModule = mainProduct.Modules.FirstOrDefault(e => string.Equals(e.Module, module.Module));
                    if (object.Equals(mainModule, null))
                    {
                        var anyActiveProduct = activeProducts.FirstOrDefault(product => product.Modules.Select(e => e.Module).Contains(module.Module));
                        if (object.Equals(anyActiveProduct, null))
                        {
                            var anyMinorProduct = minorProducts.FirstOrDefault(product => product.Modules.Select(e => e.Module).Contains(module.Module));
                            if (!object.Equals(anyMinorProduct, null))
                                activeProducts.Add(anyMinorProduct);

                        }
                    }
                }
            }

            return activeProducts.Select(e=>e.AccountProductId);
        }
        internal static List<AccountProductModuleModel> GetAllowedModules(IEnumerable<ViewAccountProduct> products, Guid accountProductId)
        {
            var modules = new List<AccountProductModuleModel>();

            var filter = products.Where(e => !e.IsDisabled && (e.AccountProductId == accountProductId || e.IsMinor));

            foreach (var product in filter)
            {
                foreach (var module in product.Modules)
                {
                    var accountProductModule = modules.FirstOrDefault(e => string.Equals(e.Module, module.Module, _stringComparison));
                    if (object.Equals(accountProductModule, null))
                        modules.Add(new AccountProductModuleModel { Module = module.Module });
                }
            }

            var eSignModule = modules.FirstOrDefault(e => "e-sign".Equals(e.Module, _stringComparison));
            if (!object.Equals(eSignModule, null))
            {
                var unlimitedEsign = filter.FirstOrDefault(e => !e.AllowedEsignCount.HasValue);
                if (object.Equals(unlimitedEsign, null))
                    eSignModule.Allowed = filter.Sum(e => e.AllowedEsignCount);

                eSignModule.Used = filter.Sum(e => e.UsedEsignCount);
            }

            return modules;
        }

        internal static List<AccountProductModuleModel> GetAllowedModules(IEnumerable<ViewAccountProduct> products, Guid accountProductId, ClaimsIdentity identity)
        {
            var modules = GetAllowedModules(products, accountProductId);

            identity.TryRemoveClaim("module");

            foreach (var module in modules)
                identity.AddClaim(new Claim("module", module.Module));

            return modules;
        }

        internal static Guid Session(HttpRequestMessage request, out AuthenticationTicket ticket)
        {
            var accessToken = request.Headers.Authorization.Parameter;
            ticket = Startup.OAuthBearerOptions.AccessTokenFormat.Unprotect(accessToken);

            return Guid.Parse(ticket.Properties.Dictionary["session"]);
        }
    }
}