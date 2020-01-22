using SaaS.Api.Core.Filters;
using System;
using System.Net.Http;
using System.Web.Http;

namespace SaaS.Api.Sign.Controllers.Api.Silanis
{
    [RoutePrefix("api/groups"), SaaSAuthorize]
    public class GroupsController : BaseApiController
    {
        protected override string ApiRoot
        {
            get { return "api/groups/"; }
        }

        [Route("{*url}"), HttpGet, HttpPost, HttpPut, HttpDelete]
        public HttpResponseMessage Index()
        {
            return HttpProxy(Request, Request.RequestUri.LocalPath);
        }

        [Route, HttpGet]
        public HttpResponseMessage Groups()
        {
            return HttpProxy(Request, Format());
        }

        [Route("{groupId:guid}"), HttpGet]
        public HttpResponseMessage GroupId(Guid groupId)
        {
            return HttpProxy(Request, Format("{0}", groupId));
        }
    }
}
