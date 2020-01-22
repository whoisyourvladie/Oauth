using SaaS.Api.Core.Filters;
using System.Net.Http;
using System.Web.Http;

namespace SaaS.Api.Sign.Controllers.Api.Silanis
{
    [RoutePrefix("api/users"), SaaSAuthorize]
    public class UsersController : BaseApiController
    {
        protected override string ApiRoot
        {
            get { return "api/users/"; }
        }

        [Route("{*url}"), HttpGet, HttpPost, HttpPut, HttpDelete]
        public HttpResponseMessage Index()
        {
            return HttpProxy(Request, Request.RequestUri.LocalPath);
        }

        [Route, HttpGet]
        public HttpResponseMessage Users()
        {
            return HttpProxy(Request, Format());
        }
    }
}
