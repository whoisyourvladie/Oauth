using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace SaaS.Api.Mc.Filters
{
    public class SaaSAnonymousAttribute : System.Web.Http.AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authorization = actionContext.Request.Headers.Authorization;
            if (object.Equals(authorization, null))
                return;

            base.OnAuthorization(actionContext);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var response = actionContext.Request.CreateResponse<dynamic>(HttpStatusCode.Unauthorized, new
            {
                error = "invalid_grant",
                error_description = "Authorization has been denied for this request."
            });

            actionContext.Response = response;
        }
    }
}