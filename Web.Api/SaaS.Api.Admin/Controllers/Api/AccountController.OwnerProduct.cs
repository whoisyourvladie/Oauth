using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Api.Models.Products;
using SaaS.Data.Entities;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Admin.Oauth;
using SaaS.Identity;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Admin.Controllers.Api
{
    public partial class AccountController : SaaSApiController
    {
        private bool IsLocalSubscription(string spId)
        {
            if (string.IsNullOrEmpty(spId))
                return false;

            return spId.StartsWith("D00000", StringComparison.InvariantCultureIgnoreCase);
        }

        [HttpGet, Route("{accountId:guid}/owner-product")]
        public async Task<IHttpActionResult> OwnerProducts(Guid accountId)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                var products = await _authProduct.OwnerProductsGetAsync(account.Id);

                return Ok(ProductConvertor.OwnerAccountProductConvertor(products));
            }, accountId);
        }

        [HttpGet, Route("{accountId:guid}/owner-product/{accountProductId:guid}")]
        public async Task<IHttpActionResult> OwnerProducts(Guid accountId, Guid accountProductId)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                var details = await _authProduct.OwnerProductDetailsGetAsync(new AccountProductPair(accountId, accountProductId));

                if (object.Equals(details, null))
                    return NotFound();

                return Ok(ProductConvertor.OwnerAccountProductConvertor(details));
            }, accountId);
        }

        [HttpPost, Route("{accountId:guid}/owner-product/{accountProductId:guid}/assign"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> OwnerProductAssign(Guid accountId, Guid accountProductId, AuthViewModel model)
        {
            try
            {
                var product = await _authProduct.OwnerProductGetAsync(new AccountProductPair(accountId, accountProductId));

                if (object.Equals(product, null) || product.IsDisabled)
                    return ProductNotFound();

                var ownerAccount = await _auth.AccountGetAsync(accountId);
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
                        var statusResult = await _authProduct.ProductAssignAsync(new AccountProductPair(targetAccount.Id, product.AccountProductId), true);
                        if (statusResult == AssignStatus.Ok)
                        {
                            var log = string.Format("Product({0}, {1}) has been assigned successfully. Customer's email is {2}.",
                                product.ProductName, product.AccountProductId, ownerAccount.Email);

                            await LogInsertAsync(log, LogActionTypeEnum.AccountProductAssign, ownerAccount.Id);
                        }

                        if (statusResult != AssignStatus.Ok)
                            return Conflict();
                    }

                    if (ownerAccount.IsBusiness)
                        await _auth.AccountMaskAsBusinessAsync(targetAccount);

                    await _auth.AccountActivateAsync(targetAccount);
                }

                product = await _authProduct.OwnerProductDetailsGetAsync(new AccountProductPair(accountId, product.AccountProductId));

                return Ok(ProductConvertor.OwnerAccountProductConvertor(product));
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }

        [HttpPost, Route("{accountId:guid}/owner-product/{accountProductId:guid}/deactivate"), SaaSAuthorize]
        public async Task<IHttpActionResult> OwnerProductDeactivate(Guid accountId, Guid accountProductId)
        {
            try
            {
                var product = await _authProduct.OwnerProductGetAsync(new AccountProductPair(accountId, accountProductId));

                if (object.Equals(product, null))
                    return ProductNotFound();

                var ownerAccount = await _auth.AccountGetAsync(accountId);

                if (!object.Equals(ownerAccount, null) && !product.IsFree && !product.IsTrial && !product.IsDisabled)
                {
                    if (!IsLocalSubscription(product.SpId) && !string.IsNullOrEmpty(product.SpId))
                    {
                        var subscription = await UpclickClient.SubscriptionDetails(product.SpId);

                        if (object.Equals(subscription, null) || object.Equals(subscription.Status, null))
                            return ErrorContent("Unknown subscription", "Upclick error. Unknown subscription.");

                        if (string.Equals(subscription.Status.Name, "Active"))
                        {
                            subscription = await UpclickClient.SubscriptionCancel(product.SpId);

                            //cuz Upclick has bug in API
                            Thread.Sleep(TimeSpan.FromSeconds(3));

                            subscription = await UpclickClient.SubscriptionDetails(product.SpId);

                            if (string.Equals(subscription.Status.Name, "Active"))
                                return ErrorContent("Subscription is active", "Upclick error. Subscription is active.");
                        }
                    }

                    await _authProduct.ProductDeactivateAsync(new AccountProductPair(accountId, product.AccountProductId));

                    var log = string.Format("Product({0}, {1}) has been deactivated successfully. Customer's email is {2}.",
                            product.ProductName, product.AccountProductId, ownerAccount.Email);

                    await LogInsertAsync(log, LogActionTypeEnum.AccountProductDeactivate, ownerAccount.Id);
                }

                return Ok();
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }

        [HttpPost, Route("{accountId:guid}/owner-product/{accountProductId:guid}/unassign/{targetAccountId:guid}")]
        public async Task<IHttpActionResult> OwnerProductUnassign(Guid accountId, Guid accountProductId, Guid targetAccountId)
        {
            try
            {
                var product = await _authProduct.OwnerProductGetAsync(new AccountProductPair(accountId, accountProductId));

                if (object.Equals(product, null) || product.IsDisabled)
                    return ProductNotFound();

                var ownerAccount = await _auth.AccountGetAsync(accountId);
                var targetAccount = await _auth.AccountGetAsync(targetAccountId);
                if (!object.Equals(ownerAccount, null) && !product.IsFree && !product.IsTrial && !product.IsDisabled)
                {
                    var statusResult = await _authProduct.ProductUnassignAsync(new AccountProductPair(targetAccount.Id, product.AccountProductId));
                    if (statusResult.Status == UnassignStatus.Ok)
                    {
                        var log = string.Format("Product({0}, {1}) has been unassigned successfully. Customer's email is {2}.",
                            product.ProductName, product.AccountProductId, ownerAccount.Email);

                        await LogInsertAsync(log, LogActionTypeEnum.AccountProductUnassign, ownerAccount.Id);
                    }
                    if (statusResult.Status != UnassignStatus.Ok)
                        return Conflict();
                }

                product = await _authProduct.OwnerProductDetailsGetAsync(new AccountProductPair(accountId, product.AccountProductId));

                return Ok(ProductConvertor.OwnerAccountProductConvertor(product));
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }

        [HttpPost, Route("{accountId:guid}/owner-product/{accountProductId:guid}/allowed")]
        public async Task<IHttpActionResult> OwnerProductAllowed(Guid accountId, Guid accountProductId, [FromUri] int allowed)
        {
            try
            {
                var product = await _authProduct.OwnerProductDetailsGetAsync(new AccountProductPair(accountId, accountProductId));

                if (object.Equals(product, null) || product.IsDisabled)
                    return ProductNotFound();

                int assignedLicenses = 0;
                if (!object.Equals(product.Accounts, null))
                    assignedLicenses = product.Accounts.Count;

                if (assignedLicenses > allowed)
                    return ErrorContent("invalid_grant", "You can't change amount of licenses. Please unassign licenses from any account.", HttpStatusCode.Conflict);

                var ownerAccount = await _auth.AccountGetAsync(accountId);
                if (!object.Equals(ownerAccount, null) && !product.IsFree && !product.IsTrial)
                {
                    var statusResult = await _authProduct.ProductAllowedSetAsync(new AccountProductPair(accountId, product.AccountProductId), allowed);

                    if (statusResult == AllowedCountStatus.Ok)
                    {
                        var log = string.Format("Amount of licenses has been changed successfully. Product({0}, {1}). Customer's email is {2}. Amount({3}=>{4}).",
                            product.ProductName, product.AccountProductId, ownerAccount.Email,
                            product.AllowedCount, allowed);

                        await LogInsertAsync(log, LogActionTypeEnum.AccountProductUnassign, ownerAccount.Id);
                    }

                    if (statusResult == AllowedCountStatus.FailCantChangeAllowedCount)
                        return ErrorContent("invalid_grant", "You can't change amount of licenses for this product.", HttpStatusCode.Conflict);

                    if (statusResult != AllowedCountStatus.Ok)
                        return Conflict();
                }

                product = await _authProduct.OwnerProductDetailsGetAsync(new AccountProductPair(accountId, product.AccountProductId));

                return Ok(ProductConvertor.OwnerAccountProductConvertor(product));
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }

        private async Task<IHttpActionResult> OwnerProductSuspend(Guid accountId, Guid accountProductId, bool suspend)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                var product = await _authProduct.OwnerProductGetAsync(new AccountProductPair(accountId, accountProductId));

                if (object.Equals(product, null) || product.IsDisabled)
                    return ProductNotFound();

                if (!product.IsRenewal || IsLocalSubscription(product.SpId))
                    return ErrorContent(string.Empty, "This product can't be suspend or resumed.");

                DateTime? nextRebillDate = null;
                var subscriptionDetails = await UpclickClient.SubscriptionDetails(product.SpId);

                if (object.Equals(subscriptionDetails, null))
                    return ErrorContent(string.Empty, "Product is not exists in Customer info(Upclick).");

                var subscription = suspend ?
                    await UpclickClient.SubscriptionSuspend(product.SpId) :
                    await UpclickClient.SubscriptionResume(product.SpId);

                if (suspend && string.Equals(subscription.Status, "Active"))
                    return ErrorContent(string.Empty, "Product can't be suspend. Product is still active in Customer info(Upclick).");

                if (!suspend && !string.Equals(subscription.Status, "Active"))
                    return ErrorContent(string.Empty, "Product can't be resumed. Product is not active in Customer info(Upclick).");

                if (string.Equals(subscription.Status, "Active"))
                    nextRebillDate = subscription.NextRebillDate;

                var pair = new AccountProductPair(accountId, product.AccountProductId);
                await _authProduct.ProductNextRebillDateSetAsync(pair, nextRebillDate);

                var log = string.Format("Product({0}, {1}) has been {2} successfully.",
                            product.ProductName, product.AccountProductId, nextRebillDate.HasValue ? "resumed" : "suspended");

                await LogInsertAsync(log, LogActionTypeEnum.AccountProductDeactivate, accountId);

                product = await _authProduct.OwnerProductDetailsGetAsync(pair);

                return Ok(ProductConvertor.OwnerAccountProductConvertor(product));

            }, accountId);
        }

        [HttpPost, Route("{accountId:guid}/owner-product/{accountProductId:guid}/suspend")]
        public async Task<IHttpActionResult> OwnerProductSuspend(Guid accountId, Guid accountProductId)
        {
            return await OwnerProductSuspend(accountId, accountProductId, true);
        }

        [HttpPost, Route("{accountId:guid}/owner-product/{accountProductId:guid}/resume")]
        public async Task<IHttpActionResult> OwnerProductResume(Guid accountId, Guid accountProductId)
        {
            return await OwnerProductSuspend(accountId, accountProductId, false);
        }

        [HttpPost, Route("{accountId:guid}/owner-product/{accountProductId:guid}/next-rebill-date")]
        public async Task<IHttpActionResult> OwnerProductNextRebillDate(Guid accountId, Guid accountProductId, [FromUri] DateTime nextRebillDate)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                var product = await _authProduct.OwnerProductGetAsync(new AccountProductPair(accountId, accountProductId));

                if (object.Equals(product, null) || product.IsDisabled || !product.IsRenewal)
                    return ProductNotFound();

                if (!product.IsRenewal || IsLocalSubscription(product.SpId))
                    return ErrorContent(string.Empty, "You can't change next rebill date for this product.");

                var subscription = await UpclickClient.SubscriptionDetails(product.SpId);
                if (object.Equals(subscription, null))
                    return ErrorContent(string.Empty, "Product is not exists in Customer info(Upclick).");

                subscription = await UpclickClient.SubscriptionUpdate(product.SpId, nextRebillDate);

                DateTime? newNextRebillDate = null;
                if (string.Equals(subscription.Status.Name, "Active") && !object.Equals(subscription.NextCycleBill, null))
                    newNextRebillDate = subscription.NextCycleBill.Date;

                var pair = new AccountProductPair(accountId, product.AccountProductId);
                if (newNextRebillDate.HasValue)
                {
                    await _authProduct.ProductNextRebillDateSetAsync(pair, newNextRebillDate);

                    var log = string.Format("Next rebill date has been changed successfully. Product({0}, {1}).",
                                product.ProductName, product.AccountProductId);

                    await LogInsertAsync(log, LogActionTypeEnum.AccountProductNextRebillDateEdit, accountId);
                }

                product = await _authProduct.OwnerProductDetailsGetAsync(pair);

                return Ok(ProductConvertor.OwnerAccountProductConvertor(product));

            }, accountId);
        }

        [HttpPost, Route("{accountId:guid}/owner-product/{accountProductId:guid}/end-date")]
        public async Task<IHttpActionResult> OwnerProductEndDate(Guid accountId, Guid accountProductId, [FromUri] DateTime endDate)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                var product = await _authProduct.OwnerProductGetAsync(new AccountProductPair(accountId, accountProductId));

                if (object.Equals(product, null) || product.IsDisabled)
                    return ProductNotFound();

                if (!IsLocalSubscription(product.SpId) || product.IsPPC)
                    return ErrorContent(string.Empty, "You can't change end date for this product.");
                
                var pair = new AccountProductPair(accountId, product.AccountProductId);
                await _authProduct.ProductEndDateSetAsync(pair, endDate);

                var log = string.Format("End date has been changed successfully. Product({0}, {1}).",
                            product.ProductName, product.AccountProductId);

                await LogInsertAsync(log, LogActionTypeEnum.AccountProductEndDateEdit, accountId);

                product = await _authProduct.OwnerProductDetailsGetAsync(pair);

                return Ok(ProductConvertor.OwnerAccountProductConvertor(product));

            }, accountId);
        }
    }
}