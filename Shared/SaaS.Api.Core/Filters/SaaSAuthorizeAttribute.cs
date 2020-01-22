using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace SaaS.Api.Core.Filters
{
    public class SaaSAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
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
