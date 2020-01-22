using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Admin.Controllers.Api
{
    public partial class AccountController : SaaSApiController
    {
        [HttpGet, Route("{accountId:guid}/external/session-token")]
        public async Task<IHttpActionResult> AccountExternalSessionToken(Guid accountId)
        {
            try
            {
                return Ok(await _auth.SessionTokenExternalHistoriesAsync(accountId));
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }
    }
}