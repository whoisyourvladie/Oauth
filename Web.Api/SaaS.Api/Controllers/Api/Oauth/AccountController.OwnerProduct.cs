using Microsoft.AspNet.Identity;
using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Api.Models.Products;
using SaaS.Data.Entities;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.View;
using SaaS.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        [HttpGet, Route("owner-products"), SaaSAuthorize]
        public async Task<IHttpActionResult> OwnerProducts()
        {
            try
            {
                var products = await _authProduct.OwnerProductsGetAsync(AccountId);

                return Ok(products.ConvertAll(ProductConvertor.OwnerAccountProductConvertor));
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpGet, Route("owner-products/{accountProductId:guid}"), SaaSAuthorize]
        public async Task<IHttpActionResult> OwnerProduct(Guid accountProductId)
        {
            try
            {
                var details = await _authProduct.OwnerProductDetailsGetAsync(CreateAccountProductPair(accountProductId));

                if (object.Equals(details, null))
                    return NotFound();

                return Ok(ProductConvertor.OwnerAccountProductConvertor(details));
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        private async Task<IHttpActionResult> OwnerProductSuspend(Guid accountProductId, bool suspend, string source = null)
        {
            return await OwnerProductExecuteAsync(async delegate (Guid accountId, ViewOwnerProduct product)
            {
                var subscription = suspend ?
                    await UpclickClient.SubscriptionSuspend(product.SpId, source) :
                    await UpclickClient.SubscriptionResume(product.SpId);

                if (object.Equals(subscription, null))
                    return BadRequest();

                DateTime? nextRebillDate = null;

                if (string.Equals(subscription.Status, "Active"))
                    nextRebillDate = subscription.NextRebillDate;

                var pair = new AccountProductPair(accountId, product.AccountProductId);

                await _authProduct.ProductNextRebillDateSetAsync(pair, nextRebillDate);
                product = await _authProduct.OwnerProductDetailsGetAsync(pair);

                await NotificationManager.ProductSuspend(accountId, product, nextRebillDate);

                return Ok(ProductConvertor.OwnerAccountProductConvertor(product));

            }, accountProductId);
        }

        [HttpGet, Route("owner-products/{accountProductId:guid}/suspend/{source?}"), SaaSAuthorize]
        public async Task<IHttpActionResult> OwnerProductSuspend(Guid accountProductId, string source = null)
        {
            return await OwnerProductSuspend(accountProductId, true, source);
        }

        [HttpGet, Route("owner-products/{accountProductId:guid}/resume"), SaaSAuthorize]
        public async Task<IHttpActionResult> OwnerProductResume(Guid accountProductId)
        {
            return await OwnerProductSuspend(accountProductId, false);
        }

        [HttpPost, Route("owner-products/{accountProductId:guid}/assign"), SaaSAuthorize, ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> OwnerProductAssign(Guid accountProductId, AuthViewModel model)
        {
            return await OwnerProductExecuteAsync(async delegate (Guid ownerAccountId, ViewOwnerProduct product)
            {
                var ownerAccount = await _auth.AccountGetAsync(ownerAccountId);
                var targetAccount = await _auth.AccountGetAsync(model.Email, isIncludeSubEmails: true);
                if (object.Equals(targetAccount, null))
                {
                    targetAccount = new Account(model.Email);

                    var result = await _auth.AccountCreateAsync(targetAccount);
                    var errorResult = GetErrorResult(result);

                    if (!object.Equals(errorResult, null))
                        return errorResult;

                    targetAccount = await _auth.AccountGetAsync(model.Email);
                }

                if (!object.Equals(ownerAccount, null))
                {
                    if (!product.IsFree && !product.IsTrial && !product.IsDisabled)
                    {
                        var status = await _authProduct.ProductAssignAsync(new AccountProductPair(targetAccount.Id, product.AccountProductId));

                        if (status == AssignStatus.Ok)
                        {
                            await NotificationManager.ProductAssigned(ownerAccountId, product, targetAccount);

                            if (ownerAccount.IsBusiness)
                                await _auth.AccountMaskAsBusinessAsync(targetAccount);

                            await _auth.AccountActivateAsync(targetAccount);
                        }

                        if (status != AssignStatus.Ok)
                            return Conflict();
                    }
                }
                product = await _authProduct.OwnerProductDetailsGetAsync(new AccountProductPair(ownerAccountId, product.AccountProductId));

                var productView = ProductConvertor.OwnerAccountProductConvertor(product);

                return Ok(productView);

            }, accountProductId);
        }

        [HttpPost, Route("owner-products/{accountProductId:guid}/unassign"), SaaSAuthorize, ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> OwnerProductUnassign(AuthIdViewModel model, Guid accountProductId)
        {
            model = model ?? new AuthIdViewModel { AccountId = Guid.Parse(User.Identity.GetUserId()) };

            return await OwnerProductExecuteAsync(async delegate (Guid accountId, ViewOwnerProduct product)
            {
                var user = await _auth.AccountGetAsync(model.AccountId);
                if (!object.Equals(user, null) && !product.IsFree && !product.IsTrial && !product.IsDisabled)
                {
                    var statusResult = await _authProduct.ProductUnassignAsync(new AccountProductPair(user.Id, product.AccountProductId));
                    if (statusResult.Status == UnassignStatus.Ok)
                        await NotificationManager.ProductUnassigned(accountId, product, user);

                    if (statusResult.Status != UnassignStatus.Ok)
                        return Conflict();
                }

                product = await _authProduct.OwnerProductDetailsGetAsync(new AccountProductPair(accountId, product.AccountProductId));

                return Ok(ProductConvertor.OwnerAccountProductConvertor(product));

            }, accountProductId, true);
        }

        [HttpPost, Route("owner-products/{accountProductId:guid}/resend-assign-invitation"), SaaSAuthorize, ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> OwnerProductResendAssignInvitation(AuthIdViewModel model, Guid accountProductId)
        {
            return await OwnerProductExecuteAsync(async delegate (Guid accountId, ViewOwnerProduct product)
            {
                var user = await _auth.AccountGetAsync(model.AccountId);
                if (!object.Equals(user, null) && !product.IsFree && !product.IsTrial && !product.IsDisabled && accountId != user.Id)
                    await NotificationManager.ProductAssigned(accountId, product, user);
                else
                    return Conflict();

                return Ok();

            }, accountProductId);
        }
    }
}