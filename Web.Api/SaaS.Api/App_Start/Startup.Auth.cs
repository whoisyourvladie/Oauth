using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SaaS.Api.Oauth.Providers;
using System;
using System.Collections.Concurrent;

namespace SaaS.Api
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthServerOptions { get; private set; }
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static IDataProtectionProvider DataProtectionProvider { get; set; }

        private readonly ConcurrentDictionary<string, string> _authenticationCodes = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        public void ConfigureOAuth(IAppBuilder app)
        {
            DataProtectionProvider = app.GetDataProtectionProvider();

            OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
#if DEBUG
                AllowInsecureHttp = true,
#endif

                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(120),
                TokenEndpointPath = new PathString("/api/token"),
                Provider = new SaaSAuthorizationServerProvider(),
                RefreshTokenProvider = new SaaSRefreshTokenProvider()
            };

            OAuthBearerOptions = new OAuthBearerAuthenticationOptions { };
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        }
    }
}