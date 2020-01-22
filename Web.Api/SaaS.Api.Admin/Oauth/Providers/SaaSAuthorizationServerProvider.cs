using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using SaaS.Data.Entities.Admin.View.Oauth;
using SaaS.Identity.Admin;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SaaS.Api.Admin.Oauth.Providers
{
    public partial class SaaSAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Run(() => { context.Validated(); });
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            ViewUser user = null;
            using (var auth = new AuthRepository())
                user = await auth.ViewUserGetAsync(context.UserName, context.Password);

            if (object.Equals(user, null))
            {
                context.SetError("invalid_grant", "Your login or password is incorrect.");
                return;
            }

            if (!user.IsActive)
            {
                context.SetError("invalid_grant", "Your account has been deactivated. Please contact your administrator.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            var properties = new Dictionary<string, string> { };

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString("N")));
            identity.AddClaims(user);

            context.Validated(new AuthenticationTicket(identity, new AuthenticationProperties(properties)));
        }
    }
}