using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Admin.Controllers.Api
{
    [RoutePrefix("api/upclick"), Authorize]
    public class UpclickController : SaaSApiController
    {
        [HttpGet, Route("products")]
        public async Task<IHttpActionResult> Products()
        {
            return Ok(await _authProduct.UpclickProductsGetAsync());
        }
    }
}
