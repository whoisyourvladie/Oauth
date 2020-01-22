using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Oauth;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        [HttpGet, Route("modules"), SaaSAuthorize]
        public async Task<IHttpActionResult> Modules()
        {
            try
            {
                var sessionProducts = await GetSessionTokenProducts();
                if (object.Equals(sessionProducts.SessionToken, null))
                    return AccountUnauthorized();

                if (!sessionProducts.SessionToken.AccountProductId.HasValue)
                    return ProductNotFound();

                var product = sessionProducts.Products.FirstOrDefault(e => e.AccountProductId == sessionProducts.SessionToken.AccountProductId.Value && !e.IsDisabled);

                if (object.Equals(product, null))
                    return ProductNotFound();

                var modules = UserManagerHelper.GetAllowedModules(sessionProducts.Products, product.AccountProductId);
                var active = UserManagerHelper.GetActiveProducts(sessionProducts.Products, modules, product.AccountProductId);

                var features = GetModuleFeatures(product, modules);

                return Ok(new
                {
                    id = product.AccountProductId,
                    email = User.Identity.Name,
                    modules = modules,
                    moduleFeatures = features,
                    activeProducts = active,
                });
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }
    }
}