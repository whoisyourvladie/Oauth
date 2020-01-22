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
        [HttpPost, Route("confirm-email"), ValidateNullModel, ValidateModel]
        public async Task<IHttpActionResult> ConfirmEmail(ConfirmEmalViewModel model)
        {
            try
            {
                var user = await _auth.AccountGetAsync(model.UserId);
                if (object.Equals(user, null))
                    return AccountNotFound();

                var result = await _auth.AccountConfirmEmailAsync(user.Id);

                var errorResult = GetErrorResult(result);

                if (!object.Equals(errorResult, null))
                    return errorResult;

                if (model.IsBusiness())
                    await _auth.AccountMaskAsBusinessAsync(user);

                await _auth.AccountActivateAsync(user);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }
    }
}