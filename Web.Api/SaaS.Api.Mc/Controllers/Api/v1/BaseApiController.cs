using SaaS.Api.Mc.Http;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Mc.Controllers.Api.v1
{
    public abstract class BaseApiController : ApiController
    {
        protected abstract string ApiRoot { get; }

        [DebuggerStepThrough]
        protected string Format(string format = null, params object[] @params)
        {
            var builder = new StringBuilder(ApiRoot);

            if (!object.Equals(format, null))
                builder.AppendFormat(format, @params);

            return builder.ToString();
        }

        protected async Task<HttpResponseMessage> HttpProxy(HttpRequestMessage request, string method, CancellationToken cancellationToken)
        {
            try
            {
                using (var client = new McHttpClient(request))
                    return await client.SendAsync(method, cancellationToken);
            }
            catch (Exception exc)
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "invalid_request",
                    error_description = exc.Message
                });
            }
        }
    }
}