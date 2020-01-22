using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using NLog;
using SaaS.Data.Entities.Oauth;
using SaaS.Identity;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SaaS.Api.Oauth.Providers
{
    public class SaaSRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static Logger _oauthRefreshTokenLogger = LogManager.GetLogger("oauth-refresh-token");

        private async Task<SessionToken> GetSessionToken(IOwinContext owinContext, Guid sessionTokenId)
        {
            SessionToken sessionToken = null;

            try
            {
                using (var auth = new AuthRepository())
                    sessionToken = await auth.SessionTokenGetAsync(sessionTokenId);

                if (object.Equals(sessionToken, null))
                {
                    var client = owinContext.Get<Client>("client");
                    var clientName = object.Equals(client, null) ? string.Empty : client.Name;
                    var userAgent = owinContext.Request.Headers.Get("User-Agent");
                    var referer = owinContext.Request.Headers.Get("Referer");

                    _oauthRefreshTokenLogger.Warn("Session token '{0}' doesn't exist. Client: {1}", 
                        sessionTokenId.ToString("N"), 
                        clientName);
                }
            }
            catch (Exception exc) { _oauthRefreshTokenLogger.Error(exc); }

            return sessionToken;
        }

        private static SessionToken CreateSessionToken(IOwinContext owinContext, AuthenticationTicket ticket, out Client client)
        {
            SessionToken sessionToken = null;

            client = owinContext.Get<Client>("client");
            if (object.Equals(client, null))
            {
                _oauthRefreshTokenLogger.Warn("Client is null");
                return sessionToken;
            }

            var session = ticket.Properties.Dictionary["session"];
            Guid sessionId;
            if (!Guid.TryParse(session, out sessionId))
            {
                _oauthRefreshTokenLogger.Warn($"Session Id '{session}' is not valid");
                return sessionToken;
            }

            var account = ticket.Identity.GetUserId();
            Guid accountId;
            if (!Guid.TryParse(account, out accountId))
            {
                _oauthRefreshTokenLogger.Warn($"Account Id '{account}' is not valid");
                return sessionToken;
            }

            var systemId = owinContext.Get<Guid?>("systemId");
            var clientVersion = owinContext.Get<string>("clientVersion");
            var externalClient = owinContext.Get<ExternalClient?>("externalClient");
            var installationID = owinContext.Get<Guid?>("installationID");

            var issued = DateTime.UtcNow;
            var expires = issued.Add(Startup.OAuthServerOptions.AccessTokenExpireTimeSpan);

            sessionToken = new SessionToken
            {
                Id = sessionId,
                AccountId = accountId,
                SystemId = systemId,
                ClientId = client.Id,
                ClientVersion = clientVersion,
                ExternalClient = externalClient,
                IssuedUtc = issued,
                ExpiresUtc = expires,
                InstallationID = installationID
            };

            var parentSessionToken = owinContext.Get<SessionToken>("parentSessionToken");
            if (!object.Equals(parentSessionToken, null))
                sessionToken.ParentId = parentSessionToken.Id;

            return sessionToken;
        }

        private static ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get { return Startup.OAuthBearerOptions.AccessTokenFormat; } }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            Client client;
            var newSessionToken = CreateSessionToken(context.OwinContext, context.Ticket, out client);
            if (object.Equals(newSessionToken, null))
                return;

            var scope = context.OwinContext.Get<string[]>("scope");
            var sessionToken = context.OwinContext.Get<SessionToken>("sessionToken");
            
            context.Ticket.Properties.IssuedUtc = newSessionToken.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = newSessionToken.ExpiresUtc;

            newSessionToken.Scope = SaaSScopeWorkerFactory.ScopeToString(scope);
            newSessionToken.ProtectedTicket = AccessTokenFormat.Protect(context.Ticket);
            
            Guid? oldSessionId = null;
            if (!object.Equals(sessionToken, null)) //come from grant_refresh
            {
                oldSessionId = sessionToken.Id;
                newSessionToken.ClientVersion = sessionToken.ClientVersion;
                newSessionToken.AccountProductId = sessionToken.AccountProductId;
                newSessionToken.ExternalClient = sessionToken.ExternalClient;
                newSessionToken.InstallationID = sessionToken.InstallationID;
            }

            using (var auth = new AuthRepository())
            {
                try
                {
                    var scopeWorker = SaaSScopeWorkerFactory.Create(scope, auth);

                    await scopeWorker.SessionTokenInsertAsync(newSessionToken, oldSessionId);
                    context.SetToken(newSessionToken.Id.ToString("N"));
                }
                catch (Exception exc)
                {
                    var builder = new StringBuilder();
                    builder.AppendLine(exc.Message);

                    builder.AppendLine($"Old session token Id: '{oldSessionId}'");
                    builder.AppendLine($"Client: '{client.Name}'");
                    builder.AppendLine($"Client version: '{newSessionToken.ClientVersion}'");

                    _oauthRefreshTokenLogger.Error(exc, builder.ToString());
                }
            }
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            Guid sessionTokenId;
            if (!Guid.TryParseExact(context.Token, "N", out sessionTokenId))
                return;

            var sessionToken = await GetSessionToken(context.OwinContext, sessionTokenId);

            if (object.Equals(sessionToken, null))
            {
                context.Response.Headers.Add("Refresh-Token-Expired", new[] { string.Format("token: {0}", sessionTokenId.ToString("N")) });
                context.Response.Headers.Add("Access-Control-Expose-Headers", new[] { "Refresh-Token-Expired" });

                return;
            }

            //TODO external provider password verification
            context.OwinContext.Set("scope", SaaSScopeWorkerFactory.StringToScope(sessionToken.Scope));
            context.OwinContext.Set("systemId", sessionToken.SystemId);

            context.OwinContext.Set("sessionToken", sessionToken);
            //context.DeserializeTicket(token.ProtectedTicket);

            var ticket = AccessTokenFormat.Unprotect(sessionToken.ProtectedTicket);

            var issued = DateTime.UtcNow;
            var expires = issued.Add(Startup.OAuthServerOptions.AccessTokenExpireTimeSpan);

            ticket.Properties.IssuedUtc = issued;
            ticket.Properties.ExpiresUtc = expires;

            context.SetTicket(ticket);
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}