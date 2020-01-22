using SaaS.Api.Core;
using System.Web.Http;

namespace SaaS.Api.Sign
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.MessageHandlers.Add(new CancelledTaskHandler());
        }
    }
}
