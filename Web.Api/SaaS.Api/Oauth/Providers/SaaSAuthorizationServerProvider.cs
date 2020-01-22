using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using SaaS.Data.Entities;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.Oauth;
using SaaS.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SaaS.Api.Oauth.Providers
{
    public partial class SaaSAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private static readonly SortedList<string, Client> _clients = new SortedList<string, Client>(StringComparer.InvariantCultureIgnoreCase);
        
        static SaaSAuthorizationServerProvider()
        {
            using (var auth = new AuthRepository())
            {
                foreach (var client in auth.ClientsGet())
                    _clients.Add(client.Name, client);

            }
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            Account user = null;
            using (var auth = new AuthRepository())
            {
                using (var authProduct = new AuthProductRepository())
                {
                    user = await GrantLocalUser(context, auth);

                    if (object.Equals(user, null))
                        return;

                    await auth.AccountNeverLoginSetAsync(user);
                    await auth.AccountVisitorIdSetAsync(user, context.OwinContext.Get<Guid?>("visitorId"));

                    var client = context.OwinContext.Get<Client>("client");
                    var externalClient = context.OwinContext.Get<ExternalClient?>("externalClient");
                    var scope = context.OwinContext.Get<string[]>("scope");
                    var scopeWorker = SaaSScopeWorkerFactory.Create(scope, context, auth, authProduct);
                    if (!await scopeWorker.ValidateDeviceAsync(user))
                        return;

                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    var properties = new Dictionary<string, string> { { "session", Guid.NewGuid().ToString("N") } };

                    if (externalClient.HasValue)
                        properties["externalClient"] = OauthManager.GetExternalClientName(externalClient.Value);

                    identity.AddClaim(ClaimTypes.NameIdentifier, user.Id);
                    identity.AddClaims(user, client);

                    context.OwinContext.Set("user", user);
                    context.Validated(new AuthenticationTicket(identity, new AuthenticationProperties(properties)));
                }
            }
        }

        private static async Task<Account> GrantResourceOwnerCredentialsToken(OAuthGrantResourceOwnerCredentialsContext context, AuthRepository auth)
        {
            var token = context.OwinContext.Get<string>("token");

            if (!string.IsNullOrEmpty(token))
            {
                Guid parentRefreshToken;
                if (Guid.TryParse(token, out parentRefreshToken))
                {
                    var sessionToken = await auth.SessionTokenGetAsync(parentRefreshToken);
                    if (!object.Equals(sessionToken, null))
                    {
                        context.OwinContext.Set("parentSessionToken", sessionToken);
                        return await auth.AccountGetAsync(sessionToken.AccountId);
                    }
                }
                else
                {
                    var tokenContext = new OAuthRequestTokenContext(context.OwinContext, token);
                    var tokenReceiveContext = new AuthenticationTokenReceiveContext(context.OwinContext, Startup.OAuthBearerOptions.AccessTokenFormat, tokenContext.Token);
                    tokenReceiveContext.DeserializeTicket(tokenReceiveContext.Token);

                    var ticket = tokenReceiveContext.Ticket;
                    if (!object.Equals(ticket, null) && ticket.Properties.ExpiresUtc.HasValue && ticket.Properties.ExpiresUtc.Value > Startup.OAuthBearerOptions.SystemClock.UtcNow)
                        return await auth.AccountGetAsync(ticket.Identity.Name);
                }
            }

            return null;
        }

        private static async Task<Account> GrantLocalUser(OAuthGrantResourceOwnerCredentialsContext context, AuthRepository auth)
        {
            var user = await GrantResourceOwnerCredentialsToken(context, auth);

            if (!object.Equals(user, null))
                return user;

            user = user ?? await auth.AccountGetAsync(context.UserName);

            if (!object.Equals(user, null) && user.IsEmptyPassword())
            {
                if (context.OwinContext.Get<ExternalClient?>("externalClient").HasValue)
                    return user;

                var sessionTokensExternalHistory = await auth.SessionTokenExternalHistoriesAsync(user.Id);
                var sessionTokenExternalHistory = sessionTokensExternalHistory.FirstOrDefault(e => !e.IsUnlinked);
                if (!object.Equals(sessionTokenExternalHistory, null))
                {
                    context.SetError("invalid_grant",
                                    string.Format("You should sign in with {0}.", sessionTokenExternalHistory.ExternalClientName));

                    return null;
                }

                if (!string.IsNullOrEmpty(user.Email))
                    context.Response.Headers.Add("User-Email", new string[] { user.Email });

                if (!string.IsNullOrEmpty(user.FirstName))
                    context.Response.Headers.Add("User-FirstName", new string[] { user.FirstName });

                if (!string.IsNullOrEmpty(user.LastName))
                    context.Response.Headers.Add("User-LastName", new string[] { user.LastName });

                context.SetError("invalid_grant", "You should create an account.");

                return null;
            }

            if (object.Equals(user, null))
            {
                user = await auth.AccountGetAsync(context.UserName, isIncludeSubEmails: true);
                if (!object.Equals(user, null))
                {
                    context.SetError("invalid_grant",
                        string.Format("You cannot sign in with this email as it is no longer associated with your account.", context.UserName));
                    return null;
                }
            }

            if (!object.Equals(user, null) &&
                (auth.PasswordIsEqual(user.Password, context.Password) || string.Equals(user.Password, context.Password, StringComparison.InvariantCultureIgnoreCase)))
                return user;

            context.SetError("invalid_grant", "The email or password is incorrect.");
            return null;
        }
    }

    public class SubscriptionSignInData
    {
        public int? AccountProductId { get; set; }
        public bool SetAsDefault { get; set; }
    }
    public class SystemSignInData
    {
        public Guid MachineKey { get; set; }
        public string MotherboardKey { get; set; }
        public string PhysicalMac { get; set; }
        public bool IsAutogeneratedMachineKey { get; set; }

        public string PcName { get; set; }
    }
}