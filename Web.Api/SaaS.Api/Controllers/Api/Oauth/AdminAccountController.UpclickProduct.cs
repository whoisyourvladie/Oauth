using SaaS.Api.Core.Filters;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Controllers.Api.Oauth
{
    public partial class AdminAccountController
    {
        [HttpGet, Route("upclick-products"), SaaSAuthorize]
        public async Task<IHttpActionResult> Products()
        {
            return Ok(await _authProduct.UpclickProductsGetAsync());
        }
    }
}