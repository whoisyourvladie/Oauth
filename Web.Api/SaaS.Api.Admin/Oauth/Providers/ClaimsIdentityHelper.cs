using SaaS.Data.Entities.Admin.View;
using SaaS.Data.Entities.Admin.View.Oauth;
using System.Linq;
using System.Security.Claims;

namespace SaaS.Api.Admin.Oauth.Providers
{
    public static class ClaimsIdentityHelper
    {
        public static void TryRemoveClaim(this ClaimsIdentity identity, string type)
        {
            var claims = identity.FindAll(type).ToArray();

            foreach (var claim in claims)
                identity.TryRemoveClaim(claim);
        }

        public static void AddClaims(this ClaimsIdentity identity, ViewUser user)
        {
            identity.TryRemoveClaim(ClaimTypes.Name);
            identity.TryRemoveClaim(ClaimTypes.Role);

            identity.AddClaim(new Claim(ClaimTypes.Name, user.Login));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
        }
    }
}