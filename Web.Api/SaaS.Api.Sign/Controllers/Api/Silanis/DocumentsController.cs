using SaaS.Api.Core.Filters;
using System.Net.Http;
using System.Web.Http;

namespace SaaS.Api.Sign.Controllers.Api.Silanis
{
    [RoutePrefix("api/documents"), SaaSAuthorize]
    public class DocumentsController : BaseApiController
    {
        protected override string ApiRoot
        {
            get { return "api/documents/"; }
        }

        [Route("{*url}"), HttpGet, HttpPost, HttpPut, HttpDelete]
        public HttpResponseMessage Index()
        {
            return HttpProxy(Request, Request.RequestUri.LocalPath);
        }

        [Route, HttpGet]
        public HttpResponseMessage Documents()
        {
            return HttpProxy(Request, Format());
        }
    }
}
