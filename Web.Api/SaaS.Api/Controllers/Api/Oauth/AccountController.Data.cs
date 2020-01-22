using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Api.Models.Products;
using SaaS.Api.Oauth;
using SaaS.ModuleFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        public class AccountDetailsProducts
        {
            public Guid? Id { get; set; }
            public AccountDetailsViewModel Details { get; set; }
            public List<AccountProductViewModel> Products { get; set; }

            public List<AccountProductModuleModel> Modules { get; set; }
            public MapModule[] ModuleFeatures { get; set; }
            public IEnumerable<Guid> ActiveProducts { get; set; }
        }

        [HttpGet, Route("data"), SaaSAuthorize]
        public async Task<IHttpActionResult> Data(bool isIncludeModules = false)
        {
            try
            {
                var sessionProducts = await GetSessionTokenProducts();
                if (object.Equals(sessionProducts.SessionToken, null))
                    return AccountUnauthorized();

                sessionProducts.Products.Sort(ProductComparer.Comparer);
                sessionProducts.Products.Reverse();

                ProductComparer.ProductOrderer(sessionProducts.Products);

                var accountDetailsProducts = new AccountDetailsProducts
                {
                    Details = await GetAccountDetails(),
                    Products = sessionProducts.Products.ConvertAll(ProductConvertor.AccountProductConvertor)
                };

                if (isIncludeModules && sessionProducts.SessionToken.AccountProductId.HasValue)
                {
                    var product = sessionProducts.Products.FirstOrDefault(e => e.AccountProductId == sessionProducts.SessionToken.AccountProductId.Value && !e.IsDisabled);

                    if (!object.Equals(product, null))
                    {
                        var modules = UserManagerHelper.GetAllowedModules(sessionProducts.Products, product.AccountProductId);
                        var active = UserManagerHelper.GetActiveProducts(sessionProducts.Products, modules, product.AccountProductId);
                        var features = GetModuleFeatures(product, modules);

                        accountDetailsProducts.Id = product.AccountProductId;
                        accountDetailsProducts.Modules = modules;
                        accountDetailsProducts.ModuleFeatures = features;
                        accountDetailsProducts.ActiveProducts = active;
                    }
                }

                return Ok(accountDetailsProducts);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }
    }
}