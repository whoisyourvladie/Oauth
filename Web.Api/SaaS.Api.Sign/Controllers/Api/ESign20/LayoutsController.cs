using SaaS.Api.Core.Filters;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Sign.Controllers.Api.eSign20
{
    [RoutePrefix("api-esign20/v1/layouts"), SaaSAuthorize]
    public class ESignLayoutsController : BaseApiController
    {
        protected override string ApiRoot
        {
            get { return "api/v1/layouts/"; }
        }

        [Route("{*url}"), HttpGet, HttpPost, HttpPut, HttpDelete]
        public async Task<HttpResponseMessage> Index(CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Request.RequestUri.LocalPath, cancellationToken);
        }

        [Route, HttpGet, HttpPost]
        public async Task<HttpResponseMessage> Layouts(CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format(), cancellationToken);
        }

        [Route("{layoutId:guid}"), HttpGet, HttpDelete]
        public async Task<HttpResponseMessage> Layouts(Guid layoutId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}", layoutId), cancellationToken);
        }
    }
}
