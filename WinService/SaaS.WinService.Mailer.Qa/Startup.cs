using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(SaaS.WinService.Mailer.Qa.Startup))]
namespace SaaS.WinService.Mailer.Qa
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
