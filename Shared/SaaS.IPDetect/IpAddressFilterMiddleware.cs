using Microsoft.Owin;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.IPDetect
{
    public class IpAddressFilterMiddleware : OwinMiddleware
    {
        static HashSet<string> _httpMethods;
        static Logger _oauthIpFilterLogger = LogManager.GetLogger("oauth-ip-filter");

        static IpAddressFilterMiddleware()
        {
            _httpMethods = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            _httpMethods.Add("Get");
            _httpMethods.Add("Put");
            _httpMethods.Add("Post");
            _httpMethods.Add("Delete");
        }

        public IpAddressFilterMiddleware(OwinMiddleware next) : base(next) { }

        public async override Task Invoke(IOwinContext context)
        {
            if (_httpMethods.Contains(context.Request.Method) && !context.IsIpAddressAllowed())
            {
                _oauthIpFilterLogger.Warn("You don't have authorization to view this page.");

                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                var error = new
                {
                    error = "invalid_grant",
                    error_description = "You don't have authorization to view this page."
                };
                var json = JsonConvert.SerializeObject(error);

                context.Response.Write(json);
                return;
            }

            await Next.Invoke(context);
        }
    }
}