using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using SaaS.Data.Entities.Admin.View.Oauth;
using SaaS.Identity.Admin;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SaaS.Api.Admin.Oauth.Providers
{
    public partial class SaaSAuthorizationServerProvider
    {
        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var identity = new ClaimsIdentity(context.Ticket.Identity);
            var properties = context.Ticket.Properties.Dictionary;

            ViewUser user = null;
            using (var auth = new AuthRepository())
                user = await auth.ViewUserGetAsync(Guid.Parse(identity.GetUserId()));

            if (object.Equals(user, null) || !user.IsActive)
                return;

            identity.AddClaims(user);

            context.Validated(new AuthenticationTicket(identity, context.Ticket.Properties));
        }
    }
}