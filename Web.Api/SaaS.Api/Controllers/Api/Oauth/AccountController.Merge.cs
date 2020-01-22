using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.View.Accounts;
using SaaS.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        [HttpPost, Route("merge-send"), ValidateNullModel, ValidateModel, SaaSAuthorize]
        public async Task<IHttpActionResult> MergeSend(MergeViewModel model)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account user)
            {
                var userFrom = await _auth.AccountGetAsync(model.AccountIdFrom);
                if (object.Equals(userFrom, null) || userFrom.Id == AccountId)
                    return AccountNotFound();

                var userPrimaryEmail = model.AccountIdPrimaryEmail == userFrom.Id ? userFrom : user;

                await NotificationManager.MergeConfirmationNotification(user, userFrom, userPrimaryEmail);

                return Ok();
            });
        }

        [HttpPost, Route("merge-confirmation")]
        public async Task<IHttpActionResult> MergeConfirmation([FromUri] Guid token)
        {
            try
            {
                var pending = await _auth.AccountMergePendingGetAsync(token);
                if (object.Equals(pending, null))
                    return AccountNotFound();

                await _auth.AccountMergeAsync(pending);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }

        [HttpPost, Route("merge-request"), ValidateNullModel, ValidateModel, SaaSAuthorize]
        public async Task<IHttpActionResult> MergeRequest(AuthViewModel model)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account user)
            {
                var userFrom = await _auth.AccountGetAsync(model.Email);
                if (object.Equals(userFrom, null) || userFrom.Id == user.Id)
                    return AccountNotFound();

                return Ok(new ViewAccountMergePending
                {
                    AccountIdTo = user.Id,
                    AccountEmailTo = user.Email,

                    AccountIdFrom = userFrom.Id,
                    AccountEmailFrom = userFrom.Email,

                    AccountIdPrimaryEmail = user.Id
                });
            });
        }

        [HttpGet, Route("merge-pending"), SaaSAuthorize]
        public async Task<IHttpActionResult> MergePending()
        {
            try { return Ok(await _auth.AccountMergePendingsGetAsync(AccountId)); }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpDelete, Route("merge-pending"), SaaSAuthorize]
        public async Task<IHttpActionResult> MergePendingDelete()
        {
            try { await _auth.AccountMergePendingDeleteAsync(AccountId); }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }
    }
}