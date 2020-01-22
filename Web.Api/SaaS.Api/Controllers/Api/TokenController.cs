using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.Owin.Security;
using SaaS.Api.Controllers.Api.Oauth;
using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Api.Oauth;
using SaaS.Api.Oauth.Providers;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Oauth;
using SaaS.Data.Entities.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api
{
    [RoutePrefix("api"), SaaSAuthorize, System.Web.Mvc.ValidateAntiForgeryToken]
    public class TokenController : SaaSApiController
    {
        internal async Task<IHttpActionResult> PostToken(List<ViewAccountProduct> products, ViewAccountProduct product,
                                                         List<SessionToken> sessions, SessionToken session,
                                                         Account user, AuthenticationTicket ticket)
        {
            if (object.Equals(product, null))
                return ProductNotFound();

            var scopeWorker = SaaSScopeWorkerFactory.Create(session.Scope, _auth, _authProduct);
            if (!scopeWorker.ValidateSessionTokenAsync(user, sessions, session, product))
                return ErrorContent("invalid_grant", "You are logged into too many devices.", HttpStatusCode.Conflict);

            if (!await scopeWorker.ValidateAccountSystemAsync(user, session, product))
                return ErrorContent("invalid_grant", "Your license is currently associated with another device.", HttpStatusCode.Conflict);

            var modules = UserManagerHelper.GetAllowedModules(products, product.AccountProductId, ticket.Identity);
            var activeProducts = UserManagerHelper.GetActiveProducts(products, modules, product.AccountProductId);

            var moduleFeatures = AccountController.GetModuleFeatures(product, modules);

            var client = GetClient(session.ClientId);
            var oldId = OauthManager.UpdateTicket(user, session, ticket, product, client);
            await _auth.SessionTokenInsertAsync(session, oldId, false, product.IsPPC);

            return Ok(OauthManager.ReleaseToken(session, ticket, modules, moduleFeatures, activeProducts, user));
        }

        [HttpPost, Route("token/auto")]
        public async Task<IHttpActionResult> PostToken()
        {
            return await CurrentAccountExecuteAsync(async delegate (Account user)
            {
                AuthenticationTicket ticket;
                var sessionId = UserManagerHelper.Session(Request, out ticket);
                var sessions = await _auth.SessionTokensGetAsync(user.Id);

                var sessionToken = sessions.FirstOrDefault(e => e.Id == sessionId);
                if (object.Equals(sessionToken, null))
                    return AccountUnauthorized();

                var products = await _authProduct.AccountProductsGetAsync(sessionToken.AccountId, sessionToken.SystemId);

                var scopeWorker = SaaSScopeWorkerFactory.Create(sessionToken.Scope);
                scopeWorker.FilterProducts(products, sessionToken);

                var offeringProducts = products.Where(e => !e.IsDisabled && !e.IsMinor && !e.IsPPC);

                var product = offeringProducts.FirstOrDefault(e => !e.IsFree) ??
                              offeringProducts.FirstOrDefault();

                return await PostToken(products, product, sessions, sessionToken, user, ticket);
            });
        }

        [HttpPost, Route("token/{accountProductId:guid}")]
        public async Task<IHttpActionResult> PostToken(Guid accountProductId, bool setAsDefault = false)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account user)
            {
                if (!user.IsActivated && !user.IsAnonymous)
                    return AccountNotActivated();

                AuthenticationTicket ticket;
                var sessionId = UserManagerHelper.Session(Request, out ticket);
                var sessions = await _auth.SessionTokensGetAsync(user.Id);

                var sessionToken = sessions.FirstOrDefault(e => e.Id == sessionId);
                if (object.Equals(sessionToken, null))
                    return AccountUnauthorized();

                var products = await _authProduct.AccountProductsGetAsync(sessionToken.AccountId, sessionToken.SystemId);

                var scopeWorker = SaaSScopeWorkerFactory.Create(sessionToken.Scope);
                scopeWorker.FilterProducts(products, sessionToken);

                var product = products.FirstOrDefault(e => e.AccountProductId == accountProductId && !e.IsDisabled);

                return await PostToken(products, product, sessions, sessionToken, user, ticket);
            });
        }

        [HttpDelete, Route("token/{refreshToken:guid}"), AllowAnonymous]
        public async Task<IHttpActionResult> DeleteToken(Guid refreshToken)
        {
            try { await _auth.SessionTokenDeleteAsync(refreshToken); }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }

        [HttpDelete, Route("token/logout"), AllowAnonymous]
        public async Task<IHttpActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                AuthenticationTicket ticket;
                try
                {
                    var session = UserManagerHelper.Session(Request, out ticket);
                    await _auth.SessionTokenDeleteAsync(session);
                }
                catch (Exception exc) { _oautLogLogger.Error(exc); }
            }

            return Ok();
        }

        [HttpPost, Route("token/jwt"), AllowAnonymous]
        public async Task<IHttpActionResult> TokenJwt(AuthPasswordViewModel model)
        {
            //TODO
            //Add client_id, client_secret ...
            //Add secret
            //Add different options as jwt, ...

            return await AccountExecuteAsync(async delegate (Account account)
            {
                if (account.IsEmptyPassword() || !_auth.PasswordIsEqual(account.Password, model.Password))
                    return AccountNotFound();

                var epochTime = (DateTime.UtcNow - new DateTime(1970, 1, 1));

                var payload = new Dictionary<string, object>()
                {
                    { "iat", (int)epochTime.TotalSeconds },
                    { "jti", Guid.NewGuid().ToString() },
                    { "name", account.UserName },
                    { "email", account.Email }
                };

                var algorithm = new HMACSHA256Algorithm();
                var serializer = new JsonNetSerializer();
                var urlEncoder = new JwtBase64UrlEncoder();
                var encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

                var key = "2MTWNIBsECQXgRIlzgLapUjb76a8ZyVVA0aF4lWuuPW0Q0kp";

#if STAGE
                key = "LsTMWq5VySJoXoOJB57LhfM0XLCaRU2w7lXVuswMBX6U3p7d";
#endif

                var jwt = encoder.Encode(payload, key);

                return Ok(new { jwt });
            }, model);
        }
    }
}