using Microsoft.AspNet.Identity;
using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using SaaS.Api.Oauth;
using SaaS.Data.Entities.Accounts;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AccountController
    {
        [HttpPost, Route("send-activation-email"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> SendActivationEmail(AuthViewModel model)
        {
            return await AccountExecuteAsync(async delegate (Account account)
            {
#if PdfForge
                var accountDetails = await _auth.AccountDetailsGetAsync(account.Id);
                if (!object.Equals(accountDetails, null) && "covermount".Equals(accountDetails.Build, StringComparison.InvariantCultureIgnoreCase))
                {
                    await NotificationManager.EmailConfirmationCovermount(account);
                    return Ok();
                }
#endif

                await NotificationManager.EmailConfirmation(account);
                return Ok();
            }, model);
        }

        [HttpPost, Route("confirm-email"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> ConfirmEmail(ConfirmEmalViewModel model)
        {
            try
            {
                var account = await _auth.AccountGetAsync(model.UserId);
                if (object.Equals(account, null))
                    return AccountNotFound();

                model.Token = HttpUtility.UrlDecode(model.Token);

                IdentityResult result = null;
                if (string.IsNullOrEmpty(model.Token)) //such bad solution, but saves a lot of time
                    result = await _auth.AccountConfirmEmailAsync(account.Id);
                else
                    result = await _auth.AccountConfirmEmailAsync(account.Id, model.Token);

                var errorResult = GetErrorResult(result);

                if (!object.Equals(errorResult, null))
                    return errorResult;

                if (model.IsBusiness())
                    await _auth.AccountMaskAsBusinessAsync(account);

                await _auth.AccountActivateAsync(account);
                await _auth.AccountVisitorIdSetAsync(account, model.VisitorId);

                var internalSignInViewModel = new SignInViewModel(account);

                return ResponseMessage(await OauthManager.InternalSignIn(internalSignInViewModel));
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }
        }
    }
}