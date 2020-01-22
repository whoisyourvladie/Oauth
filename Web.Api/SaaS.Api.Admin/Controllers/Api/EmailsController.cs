using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SaaS.Api.Admin.Controllers.Api
{
    [RoutePrefix("api/emails"), Authorize(Roles = "admin")]
    public class EmailsController : SaaSApiController
    {
        [HttpGet, Route("templates")]
        public async Task<IHttpActionResult> Templates()
        {
            return Ok(/*await _authAdmin.LogActionTypesGetAsync()*/);
        }
    }
}