using NLog;
using System;
using System.Net;
using System.Net.Http;

namespace SaaS.Api.Core
{
    public static class HttpRequestMessageExtensions
    {
        private static Logger _oauthLogLogger = LogManager.GetLogger("oauth-log");

        public static HttpResponseMessage CreateExceptionResponse(this HttpRequestMessage request,
            Exception exc,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            var response = request.CreateExceptionResponse(errorDescription: "An unexpected error has occurred. Please try again later.", statusCode: statusCode);

            if (!object.Equals(exc, null))
            {
                _oauthLogLogger.Error(exc);
                response.Headers.Add("Exc-Message", exc.Message.Replace(Environment.NewLine, string.Empty));
            }

            return response;
        }
        public static HttpResponseMessage CreateExceptionResponse(this HttpRequestMessage request,
            string error = "invalid_request",
            string errorDescription = null,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            var response = request.CreateResponse(statusCode, new
            {
                error = error,
                error_description = errorDescription
            });

            return response;
        }

        public static HttpExceptionResult HttpExceptionResult(this HttpRequestMessage request, Exception exc)
        {
            return new HttpExceptionResult(exc, request);
        }
    }
}