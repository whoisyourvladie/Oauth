using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using SaaS.Api.Mc.Filters;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SaaS.Api.Mc.Controllers.Api.v1
{
    [RoutePrefix("api/v{version:int:regex(1|2)}/messages")]
    public class MessagesController : BaseApiController
    {
        protected override string ApiRoot
        {
            get
            {
                var uri = Request.RequestUri;
                if (uri.AbsolutePath.IndexOf("api/v1/messages", StringComparison.InvariantCultureIgnoreCase) != -1)
                    return "api/v1/messages/";

                return "api/v2/messages/";
            }
        }

        [Route("{*url}"), HttpGet, HttpPost, HttpPut, HttpDelete]
        public async Task<HttpResponseMessage> Index(CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Request.RequestUri.LocalPath, cancellationToken);
        }

        private JObject InjectUserIdentity(JObject jObject, string key)
        {
            jObject.Remove(key);

            var jChild = new JObject();
            jObject.Add(key, jChild);

            return jChild;
        }
        private void InjectUserIdentity(JObject jObject)
        {
            var jUser = InjectUserIdentity(jObject, "user");
            var jIdentity = InjectUserIdentity(jUser, "identity");

            jIdentity.Add("isAuthenticated", User.Identity.IsAuthenticated);
            if (User.Identity.IsAuthenticated)
            {
                jIdentity.Add("id", User.Identity.GetUserId());
                jIdentity.Add("name", User.Identity.Name);

                var claimIdentity = User.Identity as ClaimsIdentity;
                if (!object.Equals(claimIdentity, null))
                {
                    var statusClaim = claimIdentity.FindFirstValue("status");
                    uint status;
                    if (uint.TryParse(statusClaim, out status))
                        jUser.Add("status", status);

                    var jClaims = new JArray();
                    var moduleClaims = claimIdentity.FindAll("module");
                    foreach (var claim in moduleClaims)
                    {
                        var jClaim = new JObject();
                        jClaim.Add("type", claim.Type);
                        jClaim.Add("value", claim.Value);

                        jClaims.Add(jClaim);
                    }
                    jIdentity.Add("claims", jClaims);
                }
            }
        }

        private async Task<HttpResponseMessage> InjectUserIdentity(string method, CancellationToken cancellationToken)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                var jObject = new JObject();

                string json = await Request.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(json))
                    jObject = JObject.Parse(json);

                InjectUserIdentity(jObject);

                var content = new StringContent(jObject.ToString());

                content.Headers.ContentDisposition = Request.Content.Headers.ContentDisposition;
                content.Headers.ContentType = Request.Content.Headers.ContentType;

                Request.Content = content;
            }

            return await HttpProxy(Request, method, cancellationToken);
        }

        [Route, HttpPost, SaaSAnonymousAttribute]
        public async Task<HttpResponseMessage> Messages(CancellationToken cancellationToken)
        {
            return await InjectUserIdentity(Format(), cancellationToken);
        }

        [Route("{messageId:guid}/banner"), HttpGet]
        public async Task<HttpResponseMessage> PackagesBanner(Guid messageId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/banner", messageId), cancellationToken);
        }

        [Route("{messageId:guid}/content"), HttpGet]
        public async Task<HttpResponseMessage> PackagesContent(Guid messageId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/content", messageId), cancellationToken);
        }

        [Route("{messageId:guid}/notification"), HttpGet]
        public async Task<HttpResponseMessage> PackagesNotification(Guid messageId, CancellationToken cancellationToken)
        {
            return await HttpProxy(Request, Format("{0}/notification", messageId), cancellationToken);
        }
    }
}