using SaaS.Api.Models.Products;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AdminAccountController
    {
        [HttpGet, Route("products")]
        public async Task<IHttpActionResult> Products(Guid userId)
        {
            var products = await _authProduct.AccountProductsGetAsync(userId);

            products.Sort(ProductComparer.Comparer);
            products.Reverse();

            ProductComparer.ProductOrderer(products);

            return Ok(products.ConvertAll(ProductConvertor.AccountProductConvertor));
        }
    }
}