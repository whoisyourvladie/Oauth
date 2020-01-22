using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace SaaS.Api
{
    public class Global : HttpApplication
    {
        private static readonly HashSet<string> _adminAccounts = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        static Global()
        {
            _adminAccounts.Add("azverev@lulusoftware.com");
            _adminAccounts.Add("alysenko@lulusoftware.com");
            _adminAccounts.Add("sbisch@lulusoftware.com");

            _adminAccounts.Add("jdobrofsky@lulusoftware.com");
            _adminAccounts.Add("smacdonald@lulusoftware.com");
            _adminAccounts.Add("rhuneidi@lulusoftware.com");
            _adminAccounts.Add("bmarques@lulusoftware.com");
            _adminAccounts.Add("rheadon@lulusoftware.com");

            _adminAccounts.Add("sheldon.n@upbill.com");
            _adminAccounts.Add("bogdan.zavgorodniy@upclick.com");
            _adminAccounts.Add("cchoiniere@lulusoftware.com");
            _adminAccounts.Add("hroy@lulusoftware.com");
            _adminAccounts.Add("mmitchell@sodapdf.com");
            _adminAccounts.Add("rhuerster@lulusoftware.com");
        }

        void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);


            Trace.TraceInformation("Application has been successfully started");
        }

        public void Application_OnAuthorizeRequest()
        {
            var user = HttpContext.Current.User;

            if (!user.Identity.IsAuthenticated)
                return;

            var isAdmin = _adminAccounts.Contains(user.Identity.Name);

            GenericPrincipal principal = new GenericPrincipal(user.Identity, isAdmin ? new string[] { "admin" } : new string[] { });

            HttpContext.Current.User = principal;
        }
    }
}