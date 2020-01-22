using AutoMapper;
using SaaS.Api.Core;
using SaaS.Api.Models.Products;
using SaaS.Data.Entities.View;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        [HttpGet, Route("upgrade-products/{accountProductId:guid}")]
        public async Task<IHttpActionResult> UpgradeProduct(Guid accountProductId)
        {
            try
            {
                var product = await _authProduct.UpgradeProductGetAsync(accountProductId);

                if (object.Equals(product, null))
                    return NotFound();

                return Ok(UpgradeProductConvertor(product));
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        private static UpgradeProductViewModel UpgradeProductConvertor(ViewUpgradeProduct product)
        {
            return Mapper.Map(product, new UpgradeProductViewModel());
        }
    }
}