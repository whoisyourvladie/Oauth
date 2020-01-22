using Microsoft.AspNet.Identity;
using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Api.Oauth;
using SaaS.Data.Entities.Accounts;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        [HttpPost, Route("change-password"), SaaSAuthorize, ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account user)
            {
                var errorResult = GetErrorResult(await _auth.AccountChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword));

                if (!object.Equals(errorResult, null))
                    return errorResult;

                await NotificationManager.PasswordChanged(user);

                return Ok();
            });
        }

        [HttpPost, Route("reset-password"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                var user = await _auth.AccountGetAsync(model.UserId);
                if (object.Equals(user, null))
                    return AccountNotFound();

                IdentityResult result = null;

                if (string.IsNullOrEmpty(model.Token) && user.IsEmptyPassword())
                {
                    result = await _auth.AccountPasswordSetAsync(user, model.NewPassword);
                }
                else
                {
                    model.Token = HttpUtility.UrlDecode(model.Token);
                    result = await _auth.AccountResetPasswordAsync(model.UserId, model.Token, model.NewPassword);
                }

                var errorResult = GetErrorResult(result);

                if (!object.Equals(errorResult, null))
                    return errorResult;

                if (user.IsEmptyPassword())
                    await NotificationManager.AccountCreationComplete(user);

                if (!string.IsNullOrEmpty(model.FirstName) || !string.IsNullOrEmpty(model.LastName))
                {
                    user = await _auth.AccountGetAsync(model.UserId);

                    user.FirstName = string.IsNullOrEmpty(model.FirstName) ? user.FirstName : model.FirstName;
                    user.LastName = string.IsNullOrEmpty(model.LastName) ? user.LastName : model.LastName;

                    await _auth.AccountUpdateAsync(user);
                }
                await _auth.AccountActivateAsync(user);
                await _auth.AccountVisitorIdSetAsync(user, model.VisitorId);

                user = await _auth.AccountGetAsync(model.UserId);

                var internalSignInViewModel = new SignInViewModel(user);

                return ResponseMessage(await OauthManager.InternalSignIn(internalSignInViewModel));
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }

        [HttpPost, Route("recover-password"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> RecoverPassword(AuthViewModel model)
        {
            try
            {
                var slaveUser = await _auth.AccountGetAsync(model.Email, isIncludeSubEmails: true);
                if (!object.Equals(slaveUser, null) && slaveUser.IsEmptyPassword())
                {
                    var sessionTokensExternalHistory = await _auth.SessionTokenExternalHistoriesAsync(slaveUser.Id);
                    var sessionTokenExternalHistory = sessionTokensExternalHistory.FirstOrDefault(e => !e.IsUnlinked);
                    if (!object.Equals(sessionTokenExternalHistory, null))
                        return ErrorContent("invalid_grant", string.Format("You should sign in with {0}.", sessionTokenExternalHistory.ExternalClientName));
                }

                var masterUser = await _auth.AccountGetAsync(model.Email);
                if (object.Equals(masterUser, null) || masterUser.IsAnonymous)
                    return AccountNotFound();

                await NotificationManager.RecoverPassword(masterUser);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }
    }
}