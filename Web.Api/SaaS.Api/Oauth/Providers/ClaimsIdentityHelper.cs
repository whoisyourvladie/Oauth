using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Oauth;
using SaaS.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SaaS.Api.Oauth.Providers
{
    public static class ClaimsIdentityHelper
    {
        private static readonly HashSet<string> _specialClaims = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "email", "firstName", "lastName", "isAnonymous", "isActivated", "status"
        };

        public static void AddClaim(this ClaimsIdentity identity, string type, object value)
        {
            identity.AddClaim(new Claim(type, value.ToString()));
        }

        public static void TryRemoveClaim(this ClaimsIdentity identity, string type)
        {
            var claims = identity.FindAll(type).ToArray();

            foreach (var claim in claims)
                identity.TryRemoveClaim(claim);
        }

        public static void AddClaims(this ClaimsIdentity identity, Account user, Client client)
        {
            identity.TryRemoveClaim(ClaimTypes.Name);
            identity.AddClaim(ClaimTypes.Name, user.Email);

            foreach (var specialClaim in _specialClaims)
            {
                var claim = identity.FindFirst(specialClaim);

                if (!object.Equals(claim, null))
                    identity.TryRemoveClaim(claim);
            }

            string[] modules = null;

            if (user.IsAnonymous)
            {
                identity.TryRemoveClaim("module");
                switch (client.Name.ToLower())
                {
                    case "saas": modules = new string[] { "create", "free convert", "paywall" }; break;
                    case "pdfrotate": modules = new string[] { "edit" }; break;
                    case "pdfprotect": modules = new string[] { "secure" }; break;
                    case "splitpdf": modules = new string[] { "edit" }; break;
                    case "pdfconvert": modules = new string[] { "convert" }; break;
                    case "pdfcreateconvert": modules = new string[] { "create", "convert", "edit", "insert", "secure" }; break;

                    case "pdfmerge":
                    case "pdfcombine":
                    case "pdfjoin":
                    case "pdfcreate":
                    case "esign-lite":
                    case "soda-lite":
                        modules = new string[] { "create", }; break;

                    case "soda-pdf-3d-reader": modules = new string[] { "create", "free convert" }; break;
                }
            }
            else
            {
                if ("web".Equals(client.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    identity.TryRemoveClaim("module");
                    modules = new string[] { "create", "convert", "edit", "insert", "secure" };
                }
            }

            if ("mobile-pdf-merge".Equals(client.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                identity.TryRemoveClaim("module");
                modules = new string[] { "create" };
            }

            if (!object.Equals(modules, null))
            {
                foreach (var module in modules)
                    identity.AddClaim(new Claim("module", module));
            }


            if ("soda-lite".Equals(client.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                var sodaLiteClaimName = "paywall-once-per-day";
                if (!identity.HasClaim(x => x.Type == "module" && x.Value == sodaLiteClaimName))
                    identity.AddClaim(new Claim("module", sodaLiteClaimName));
            }

            identity.AddClaim("status", user.GetStatus());
        }
    }
}