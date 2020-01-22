using SaaS.Api.Core.Filters;
using System.Net.Http;
using System.Web.Http;

namespace SaaS.Api.Sign.Controllers.Api.Silanis
{
    [RoutePrefix("api/layouts"), SaaSAuthorize]
    public class LayoutsController : BaseApiController
    {
        protected override string ApiRoot
        {
            get { return "api/layouts/"; }
        }

        [Route("{*url}"), HttpGet, HttpPost, HttpPut, HttpDelete]
        public HttpResponseMessage Index()
        {
            return HttpProxy(Request, Request.RequestUri.LocalPath);
        }

        [Route, HttpGet, HttpPost]
        public HttpResponseMessage Layouts()
        {
            return HttpProxy(Request, Format());
        }
    }
}
