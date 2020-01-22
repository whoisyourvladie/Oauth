using SaaS.Api.Core;
using SaaS.Api.Core.Filters;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using ValidateAntiForgeryToken = System.Web.Mvc.ValidateAntiForgeryTokenAttribute;

namespace SaaS.Api.Controllers.Api.Oauth
{
    [RoutePrefix("api/admin/account"), ValidateAntiForgeryToken, SaaSAuthorize(Roles = "admin")]
    public partial class AdminAccountController : SaaSApiController
    {
        [HttpDelete, Route]
        public async Task<IHttpActionResult> Delete(Guid userId)
        {
            try
            {
                var user = await _auth.AccountGetAsync(userId);

                if (object.Equals(user, null))
                    return AccountNotFound();

                await _auth.AccountDeleteAsync(user);
            }
            catch (Exception exc) { return Request.HttpExceptionResult(exc); }

            return Ok();
        }
    }
}