using SaaS.Api.Admin.Models.Oauth;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Admin.Oauth;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Admin.Controllers.Api
{
    [RoutePrefix("api/account"), Authorize]
    public partial class AccountController : SaaSApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> Index(string filter = null, string globalOrderId = null, string email = null, string transactionOrderUid = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    return await CurrentAccountExecuteAsync(async delegate (Account account)
                    {
                        return await Task.FromResult(Ok(new AccountViewModel(account)));

                    }, email, isIncludeSubEmails: true);
                }

                if (!string.IsNullOrEmpty(transactionOrderUid))
                {
                    var account = await _auth.AccountGetByTransactionOrderUidAsync(transactionOrderUid);
                    if (object.Equals(account, null))
                        return AccountNotFound();

                    return Ok(new AccountViewModel(account));
                }

                var accounts = await _auth.AccountsGetAsync(filter, globalOrderId);

                return Ok(accounts.Select(account => new AccountViewModel(account)));
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }

        [HttpPut, ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> AccountPut(AuthNameViewModel model)
        {
            try
            {
                var account = await _auth.AccountGetAsync(model.Email, isIncludeSubEmails: true);

                if (!object.Equals(account, null))
                    return AccountExists();

                account = new Account(model.Email, model.FirstName, model.LastName);

                var errorResult = GetErrorResult(await _auth.AccountCreateAsync(account));

                if (!object.Equals(errorResult, null))
                    return errorResult;

                account = await _auth.AccountGetAsync(model.Email);

                var log = string.Format("Customer({0}) has been created.", account.Email);

                await LogInsertAsync(log, LogActionTypeEnum.AccountCreate, account.Id);

                return Ok(new AccountViewModel(account));
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }

        [HttpGet, Route("{accountId:guid}")]
        public async Task<IHttpActionResult> AccountGet(Guid accountId)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                return await Task.FromResult(Ok(new AccountViewModel(account)));

            }, accountId);
        }

        [HttpGet, Route("{accountId:guid}/uid")]
        public async Task<IHttpActionResult> AccountUidGet(Guid accountId)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                var uid = await _auth.AccountUidGetAsync(accountId);

                return Ok(new { uid });

            }, accountId);
        }

        [HttpPost, Route("{accountId:guid}"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> AccounPost(Guid accountId, AuthNameViewModel model)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                var userWithEmail = await _auth.AccountGetAsync(model.Email, isIncludeSubEmails: true);

                if (!object.Equals(userWithEmail, null) && account.Id != userWithEmail.Id)
                    return AccountExists();

                if (!string.Equals(account.FirstName, model.FirstName, StringComparison.InvariantCultureIgnoreCase) ||
                   !string.Equals(account.LastName, model.LastName, StringComparison.InvariantCultureIgnoreCase))
                {
                    account.FirstName = model.FirstName;
                    account.LastName = model.LastName;

                    var errorResult = GetErrorResult(await _auth.AccountUpdateAsync(account));
                    if (!object.Equals(errorResult, null))
                        return errorResult;

                    var log = string.Format("Customer's({0}) first or last name have been changed.", account.Email);

                    await LogInsertAsync(log, LogActionTypeEnum.AccountEdit, account.Id);
                }

                if (!string.Equals(account.Email, model.Email, StringComparison.InvariantCultureIgnoreCase))
                {
                    account.Email = model.Email;

                    await _auth.AccountSubEmailPendingDeleteAsync(account.Id);

                    var pending = await _auth.AccountSubEmailPendingSetAsync(account.Id, model.Email);
                    await _auth.AccountEmailSetAsync(pending);

                    var log = string.Format("Customer's({0}) email has been changed.", account.Email);

                    await LogInsertAsync(log, LogActionTypeEnum.AccountEdit, account.Id);
                }

                return Ok(new AccountViewModel(account));

            }, accountId);
        }

        [HttpDelete, Route("{accountId:guid}")]
        public async Task<IHttpActionResult> AccountDelete(Guid accountId)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                await _auth.AccountDeleteAsync(accountId);

                var log = string.Format("Customer({0}) has been deleted.", account.Email);

                await LogInsertAsync(log, LogActionTypeEnum.AccountDelete, account.Id);

                return Ok();

            }, accountId);
        }

        [HttpDelete, Route("gdpr/{accountId:guid}")]
        public async Task<IHttpActionResult> AccountGDPRDelete(Guid accountId)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                
                var result = await _gdpr.AccountGDPRDeleteAsync(account.Email);
                if (!result.Equals("success", StringComparison.InvariantCultureIgnoreCase)) {
                    return BadRequest(result);
                }
                   

                var log = $"Customer({account.Email}) has been deleted by GDPR.";

                await LogInsertAsync(log, LogActionTypeEnum.AccountDelete, account.Id);

                return Ok();

            }, accountId);
        }

        [HttpPost, Route("{accountId:guid}/confirm-email")]
        public async Task<IHttpActionResult> ConfirmEmail(Guid accountId, string build)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                var errorResult = GetErrorResult(await _auth.AccountConfirmEmailAsync(accountId));

                if (!object.Equals(errorResult, null))
                    return errorResult;

                if ("b2b".Equals(build, StringComparison.InvariantCultureIgnoreCase))
                {
                    await _auth.AccountMaskAsBusinessAsync(account);
                    await LogInsertAsync(string.Format("Customer({0}) has been marked as business.", account.Email), LogActionTypeEnum.AccountMaskBusiness, account.Id);
                }

                await _auth.AccountActivateAsync(account);
                await LogInsertAsync(string.Format("Customer({0}) has been activated.", account.Email), LogActionTypeEnum.AccountActivate, account.Id);

                account = await _auth.AccountGetAsync(accountId);

                return Ok(new AccountViewModel(account));

            }, accountId);
        }

        [HttpPost, Route("{accountId:guid}/change-password"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> ChangePassword(Guid accountId, CustomerChangePasswordViewModel model)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                var errorResult = GetErrorResult(await _auth.AccountPasswordSetAsync(account, model.NewPassword));

                if (!object.Equals(errorResult, null))
                    return errorResult;

                var log = string.Format("Customer's({0}) password has been changed.", account.Email);

                await LogInsertAsync(log, LogActionTypeEnum.AccountChangePassword, account.Id);

                return Ok();

            }, accountId);
        }

        [HttpPost, Route("{accountId:guid}/recover-password"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> RecoverPassword(Guid accountId)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                await NotificationManager.RecoverPassword(account);

                return Ok();
            }, accountId);
        }
    }
}