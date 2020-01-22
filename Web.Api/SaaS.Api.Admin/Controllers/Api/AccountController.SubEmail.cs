using SaaS.Data.Entities.Accounts;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Admin.Controllers.Api
{
    public partial class AccountController : SaaSApiController
    {
        [HttpGet, Route("{accountId:guid}/sub-email")]
        public async Task<IHttpActionResult> AccountSubEmail(Guid accountId)
        {
            try
            {
                return Ok(await _auth.AccountSubEmailsGetAsync(accountId));
            }
            catch (Exception exc) { return ErrorContent(exc); }
        }

        [HttpDelete, Route("{accountId:guid}/sub-email/{id:int}")]
        public async Task<IHttpActionResult> AccountSubEmailDelete(Guid accountId, int id)
        {
            return await CurrentAccountExecuteAsync(async delegate (Account account)
            {
                await _auth.AccountSubEmailDeleteAsync(id);
                return Ok();

            }, accountId);
        }
    }
}