using SaaS.Api.Core.Filters;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Sign.Controllers.Api.eSign20
{
    [RoutePrefix("api-esign20/v1/signers"), SaaSAuthorize]
    public class ESignSignersController : BaseApiController
    {
        protected override string ApiRoot
        {
            get { return "api/v1/signers/"; }
        }

        [Route("{*url}"), HttpGet, HttpPost, HttpPut, HttpDelete]
        public async Task<HttpResponseMessage> Index(CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Request.RequestUri.LocalPath, cancellationToken);
        }

        [Route, HttpGet, HttpPost]
        public async Task<HttpResponseMessage> Signers(CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format(), cancellationToken);
        }

        [Route("{id:guid}"), HttpPut, HttpDelete]
        public async Task<HttpResponseMessage> Signers(Guid id, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}", id), cancellationToken);
        }

        [Route("order"), HttpPut]
        public async Task<HttpResponseMessage> Order(CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("order"), cancellationToken);
        }
    }
}
