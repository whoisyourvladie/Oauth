using Microsoft.AspNet.Identity;
using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using SaaS.Api.Models.Oauth;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AdminAccountController
    {
        [HttpPost, Route("change-password"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> ChangePassword(ResetPasswordViewModel model)
        {
            try
            {
                var user = await _auth.AccountGetAsync(model.UserId);
                if (object.Equals(user, null))
                    return AccountNotFound();

                var result = await _auth.AccountPasswordSetAsync(user, model.NewPassword);

                var errorResult = GetErrorResult(result);

                if (!object.Equals(errorResult, null))
                    return errorResult;
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }

        [HttpPost, Route("recover-password")]
        public async Task<IHttpActionResult> RecoverPassword(Guid userId)
        {
            try
            {
                var user = await _auth.AccountGetAsync(userId);
                if (object.Equals(user, null))
                    return AccountNotFound();

                await NotificationManager.RecoverPassword(user);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }
    }
}