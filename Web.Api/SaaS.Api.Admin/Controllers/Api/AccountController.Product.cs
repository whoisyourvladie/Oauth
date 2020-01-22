using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Products;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Admin.Oauth;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Admin.Controllers.Api
{
    public partial class AccountController : SaaSApiController
    {
        [HttpPut, Route("{accountId:guid}/product"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> AccountProductPut(Guid accountId, AddOwnerProductViewModel model)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                await _authProduct.OwnerProductInsertAsync(accountId, model.ProductUid, model.Currency, model.Price, model.PriceUsd, model.Quantity);

                var log = string.Format("Product({0}) has been added successfully. Customer's email is {1}.", model.ProductUid, account.Email);

                await LogInsertAsync(log, LogActionTypeEnum.AccountProductAdd, account.Id);

                return Ok();

            }, accountId);
        }
    }
}