using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using SaaS.Data.Entities.Oauth;
using SaaS.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SaaS.Api.Oauth.Providers
{
    public partial class SaaSAuthorizationServerProvider
    {
        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var identity = new ClaimsIdentity(context.Ticket.Identity);
            var properties = context.Ticket.Properties.Dictionary;

            var client = context.OwinContext.Get<Client>("client");
            var sessionToken = context.OwinContext.Get<SessionToken>("sessionToken");

            properties["session"] = Guid.NewGuid().ToString("N");

            if (!string.IsNullOrEmpty(sessionToken.ExternalClientName))
                properties["externalClient"] = sessionToken.ExternalClientName;

            string[] scope = context.OwinContext.Get<string[]>("scope");

            var accountId = Guid.Parse(identity.GetUserId());

            using (var auth = new AuthRepository())
            {
                using (var authProduct = new AuthProductRepository())
                {
                    var account = await auth.AccountGetAsync(accountId);
                    if (object.Equals(account, null))
                        return;

                    context.OwinContext.Set("user", account);

                    identity.TryRemoveClaim("module");
                    identity.AddClaims(account, client);

                    var scopeWorker = SaaSScopeWorkerFactory.Create(scope, context, auth, authProduct);
                    await scopeWorker.GrantClaims(identity);
                }
            }

            context.Validated(new AuthenticationTicket(identity, context.Ticket.Properties));
        }
    }
}