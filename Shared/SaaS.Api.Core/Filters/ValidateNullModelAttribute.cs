using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SaaS.Api.Core.Filters
{
    public class ValidateNullModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            foreach (var item in actionContext.ActionArguments)
            {
                if (!object.Equals(item.Value, null))
                    return;
            }

            actionContext.Response =  actionContext.Request.CreateExceptionResponse(errorDescription: "Model is required.");
        }
    }
}