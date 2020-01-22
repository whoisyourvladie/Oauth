using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SaaS.Api.Core.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                string errorDescription = null;
                foreach (var value in actionContext.ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errorDescription = error.ErrorMessage;
                        break;
                    }

                    if (!string.IsNullOrEmpty(errorDescription))
                        break;
                }

                actionContext.Response = actionContext.Request.CreateExceptionResponse(errorDescription: errorDescription);
            }
        }
    }
}