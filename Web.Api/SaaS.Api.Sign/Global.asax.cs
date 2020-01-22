using System.Net;
using System.Web.Http;

namespace SaaS.Api.Sign
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
