using Microsoft.Owin;
using Owin;
using System.Net;

[assembly: OwinStartup(typeof(SaaS.UI.Admin.Startup))]
namespace SaaS.UI.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ConfigureOAuth(app);
        }
    }
}