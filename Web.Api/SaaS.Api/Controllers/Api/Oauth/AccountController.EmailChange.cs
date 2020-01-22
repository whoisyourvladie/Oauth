using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Data.Entities.Accounts;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        [HttpPost, Route("email-change-send"), ValidateNullModel, ValidateModel, SaaSAuthorize]
        public async Task<IHttpActionResult> EmailChangeSend(AuthViewModel model)
        {
            try
            {
                var slaveUser = await _auth.AccountGetAsync(model.Email, isIncludeSubEmails: true);
                if (!object.Equals(slaveUser, null) && slaveUser.Id != AccountId)
                    return AccountExists();

                var masterUser = await GetAccountAsync();
                if (string.Equals(masterUser.Email, model.Email, StringComparison.InvariantCultureIgnoreCase))
                    await _auth.AccountSubEmailPendingDeleteAsync(masterUser.Id);
                else
                    await NotificationManager.EmailChangeConfirmationNotification(masterUser, model.Email);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }

        [HttpPost, Route("email-change-confirmation")]
        public async Task<IHttpActionResult> EmailChangeConfirmation([FromUri] Guid token)
        {
            try
            {
                var pending = await _auth.AccountSubEmailPendingGetAsync(token);
                if (object.Equals(pending, null))
                    return AccountNotFound();

                var slave = await _auth.AccountGetAsync(pending.Email, true);
                if (!object.Equals(slave, null) && slave.Id != pending.AccountId)
                    return AccountExists();

                var masterUser = await _auth.AccountGetAsync(pending.AccountId);

                await _auth.AccountActivateAsync(masterUser);
                await _auth.AccountEmailSetAsync(pending);
                await NotificationManager.EmailChange(masterUser, pending.Email);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }

        [HttpGet, Route("email-change-pending"), SaaSAuthorize]
        public async Task<IHttpActionResult> EmailChangePending()
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                var pendings = await _auth.AccountSubEmailPendingsGetAsync(AccountId);
                var emailChangePendingViewModel = new EmailChangePendingViewModel
                {
                    Email = account.Email,
                    PendingEmails = pendings.Select(e => e.Email).ToArray()
                };

                return Ok(emailChangePendingViewModel);
            });
        }

        [HttpDelete, Route("email-change-pending"), SaaSAuthorize]
        public async Task<IHttpActionResult> EmailChangePendingDelete()
        {
            try { await _auth.AccountSubEmailPendingDeleteAsync(AccountId); }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }
    }
}