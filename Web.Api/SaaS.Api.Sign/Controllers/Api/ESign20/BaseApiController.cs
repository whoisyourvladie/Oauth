using eSign20.Api.Client;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using SaaS.Api.Core;
using SaaS.Data.Entities.eSign;
using SaaS.Data.Entities.Oauth;
using SaaS.Data.Entities.View;
using SaaS.Identity;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Sign.Controllers.Api.eSign20
{
    public abstract class BaseApiController : ApiController
    {
        protected readonly IAuthRepository _auth;
        protected readonly IeSignRepository _eSign;
        protected readonly IAuthProductRepository _authProduct;

        protected BaseApiController()
        {
            _auth = new AuthRepository();
            _eSign = new eSignRepository();
            _authProduct = new AuthProductRepository();
        }

        protected virtual string ApiRoot { get { throw new NotImplementedException(); } }
        internal virtual eSignClient eSignClient { get { return eSignClient.eSign20; } }

        [DebuggerStepThrough]
        protected string Format(string format = null, params object[] @params)
        {
            var builder = new StringBuilder(ApiRoot);

            if (!object.Equals(format, null))
                builder.AppendFormat(format, @params);

            return builder.ToString();
        }

        protected async Task<SessionToken> GetSessionToken()
        {
            var accessToken = Request.Headers.Authorization.Parameter;
            var ticket = Startup.OAuthBearerOptions.AccessTokenFormat.Unprotect(accessToken);
            var sessionId = Guid.Parse(ticket.Properties.Dictionary["session"]);

            return await _auth.SessionTokenGetAsync(sessionId);
        }

        private ulong GetContentLength(string header)
        {
            ulong length = 0;

            if (!object.Equals(Request.Headers, null) && Request.Headers.Contains(header))
            {
                var values = Request.Headers.GetValues(header);
                var fileContentLength = values.FirstOrDefault(e => !string.IsNullOrEmpty(e));

                ulong.TryParse(fileContentLength, out length);
            }

            return length;
        }
        protected ulong GetFileContentLength()
        {
            return GetContentLength("File-Content-Length");
        }
        protected ulong GetRecipientsCount(string json)
        {
            var jObject = JObject.Parse(json);
            var jRecipients = jObject.SelectToken("$.recipients", false);

            var array = jRecipients as JArray;
            if (object.Equals(array, null))
                return GetContentLength("Recipient-Content-Length");

            return (ulong)array.Count;            
        }
        protected HttpResponseMessage IncludeHeaders(HttpResponseMessage response, int? allowed = null, int? used = null)
        {
            if (allowed.HasValue)
                response.Headers.Add("Allowed", allowed.ToString());

            if (used.HasValue)
                response.Headers.Add("Used", used.ToString());

            return response;
        }

        protected HttpResponseMessage PaymentRequired(string errorDescription, int? allowed = null, int? used = null)
        {
            var response = Request.CreateResponse<dynamic>(HttpStatusCode.PaymentRequired,
                    new { error = "invalid_request", error_description = errorDescription });

            IncludeHeaders(response, allowed, used);

            return response;
        }

        protected async Task<HttpResponseMessage> HttpProxy(HttpRequestMessage request, string method, CancellationToken cancellationToken)
        {
            const string pref = "/api-esign20";

            if (!string.IsNullOrEmpty(method) && method.StartsWith(pref, StringComparison.InvariantCultureIgnoreCase))
                method = method.Remove(0, pref.Length);

            try
            {
                var accountId = Guid.Parse(User.Identity.GetUserId());
                var entity = await _eSign.eSignApiKeyGetAsync(accountId, eSignClient);
                var singIn = new SignInResult(object.Equals(entity, null) ? null : entity.Key);

                if (singIn.IsEmptyApiKey)
                {
                    singIn = await SignIn(request, accountId, cancellationToken);
                    if (!singIn.IsSuccessStatusCode)
                        return singIn.Response;
                }

                using (var client = new eSign20Client(request, singIn.ApiKey))
                {
                    var response = await client.SendAsync(method, cancellationToken);
                    if (response.StatusCode != HttpStatusCode.Unauthorized)
                        return response;
                }

                //TODO if revoke
                singIn = await SignIn(request, accountId, cancellationToken);
                if (!singIn.IsSuccessStatusCode)
                    return singIn.Response;

                using (var client = new eSign20Client(request, singIn.ApiKey))
                {
                    var response = await client.SendAsync(method, cancellationToken);
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                        response.StatusCode = HttpStatusCode.ProxyAuthenticationRequired;

                    return response;
                }
            }
            catch (Exception exc) { return request.CreateExceptionResponse(exc); }
        }

        private async Task<SignInResult> SignIn(HttpRequestMessage request, Guid accountId, CancellationToken cancellationToken)
        {
            var account = await _auth.AccountGetAsync(accountId);
            var response = await eSign20Client.SenderApiKeyAsync(account.Email, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return new SignInResult(response);

            var jObject = await response.Content.ReadAsAsync<JObject>();

            var apiKey = jObject.Value<string>("apiKey");

            if (string.IsNullOrEmpty(apiKey))
            {
                return new SignInResult(request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = "invalid_request",
                    error_description = "Api key is empty."
                }));
            }

            await _eSign.eSignApiKeySetAsync(account.Id, account.Email, eSignClient, apiKey);

            return new SignInResult(apiKey);
        }
        protected override void Dispose(bool disposing)
        {
            _auth.Dispose();
            _eSign.Dispose();
            _authProduct.Dispose();

            base.Dispose(disposing);
        }

        private class SignInResult
        {
            public SignInResult(HttpResponseMessage response) : this(response, null) { }
            public SignInResult(string apiKey) : this(new HttpResponseMessage(HttpStatusCode.OK), apiKey) { }
            public SignInResult(HttpResponseMessage response, string apiKey)
            {
                Response = response;
                ApiKey = apiKey;
            }

            internal HttpResponseMessage Response { get; private set; }
            internal string ApiKey { get; private set; }

            internal bool IsSuccessStatusCode { get { return Response.IsSuccessStatusCode; } }
            internal bool IsEmptyApiKey { get { return string.IsNullOrEmpty(ApiKey); } }
        }
    }
}