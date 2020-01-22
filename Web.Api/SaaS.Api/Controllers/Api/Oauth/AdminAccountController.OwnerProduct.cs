using SaaS.Api.Models.Products;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AdminAccountController
    {
        [HttpGet, Route("owner-products")]
        public async Task<IHttpActionResult> OwnerProducts(Guid userId)
        {
            var products = await _authProduct.OwnerProductsGetAsync(userId);

            return Ok(ProductConvertor.OwnerAccountProductConvertor(products));
        }

        [HttpPost, Route("owner-products")]
        public async Task<IHttpActionResult> OwnerProductInsert(AddOwnerProductViewModel model)
        {
            await _authProduct.OwnerProductInsertAsync(model.UserId, model.ProductUid, model.Currency, model.Price, model.PriceUsd, model.Quantity);

            return Ok();
        }
    }
}