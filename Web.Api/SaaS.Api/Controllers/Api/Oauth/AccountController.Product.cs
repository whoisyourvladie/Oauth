using Microsoft.Owin.Security;
using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Products;
using SaaS.Api.Oauth;
using SaaS.Api.Oauth.Providers;
using SaaS.Data.Entities.Oauth;
using SaaS.Data.Entities.View;
using SaaS.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        private class SessionTokenProducts
        {
            internal SessionTokenProducts() { }
            internal SessionTokenProducts(SessionToken sessionToken, List<ViewAccountProduct> products)
            {
                SessionToken = sessionToken;
                Products = products;
            }

            internal SessionToken SessionToken { get; set; }
            internal List<ViewAccountProduct> Products { get; set; }
        }

        private async Task<SessionTokenProducts> GetSessionTokenProducts()
        {
            AuthenticationTicket ticket;
            var session = UserManagerHelper.Session(Request, out ticket);
            var sessionToken = await _auth.SessionTokenGetAsync(session);

            if (object.Equals(sessionToken, null))
                return new SessionTokenProducts();

            var products = await _authProduct.AccountProductsGetAsync(sessionToken.AccountId, sessionToken.SystemId);

            var scopeWorker = SaaSScopeWorkerFactory.Create(sessionToken.Scope);
            scopeWorker.FilterProducts(products, sessionToken);

            return new SessionTokenProducts(sessionToken, products);
        }

        [HttpGet, Route("products"), SaaSAuthorize]
        public async Task<IHttpActionResult> Products()
        {
            try
            {
                var sessionProducts = await GetSessionTokenProducts();
                if (object.Equals(sessionProducts.SessionToken, null))
                    return AccountUnauthorized();

                sessionProducts.Products.Sort(ProductComparer.Comparer);
                sessionProducts.Products.Reverse();

                ProductComparer.ProductOrderer(sessionProducts.Products);

                return Ok(sessionProducts.Products.ConvertAll(ProductConvertor.AccountProductConvertor));
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpGet, Route("products/{accountProductId:guid}"), SaaSAuthorize]
        public async Task<IHttpActionResult> Products(Guid accountProductId)
        {
            try
            {
                AuthenticationTicket ticket;
                var session = UserManagerHelper.Session(Request, out ticket);
                var sessionToken = await _auth.SessionTokenGetAsync(session);
                if (object.Equals(sessionToken, null))
                    return Unauthorized();

                int usedPerSystem = 0;

                if (sessionToken.SystemId.HasValue)
                {
                    var accountSystems = await _auth.AccountSystemsGetAsync(new AccountProductPair(sessionToken.AccountId, accountProductId));
                    usedPerSystem = accountSystems.Count(e => e.SystemId == sessionToken.SystemId.Value);
                }
                return Ok(new { usedPerSystem = usedPerSystem });
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpDelete, Route("products/{accountProductId:guid}/notification"), SaaSAuthorize]
        public async Task<IHttpActionResult> ProductsDeleteNotification(Guid accountProductId)
        {
            try
            {
                var pair = CreateAccountProductPair(accountProductId);

                await _authProduct.ProductIsNewAsync(pair, false);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }
    }
}