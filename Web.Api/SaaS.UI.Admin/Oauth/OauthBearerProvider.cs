using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using SaaS.Api.Client;
using System;
using System.Threading.Tasks;
using System.Web;

namespace SaaS.UI.Admin.Oauth
{
    public class OauthBearerProvider : IOAuthBearerAuthenticationProvider
    {
        private ISaaSApiOauthService _oauthService;
        private static readonly CookieOptions _cookieOptions = new CookieOptions { Expires = DateTime.Now.AddYears(10) };

        public OauthBearerProvider(ISaaSApiOauthService oauthService)
        {
            _oauthService = oauthService;
        }

        public Task ApplyChallenge(OAuthChallengeContext context)
        {
            context.Response.Redirect("/user/login");
            return Task.FromResult<object>(null);
        }

        public async Task RequestToken(OAuthRequestTokenContext context)
        {
            string requestAccessToken = HttpContext.Current.Request.QueryString["access_token"];
            string requestRefreshToken = HttpContext.Current.Request.QueryString["refresh_token"];

            string cookieAccessToken = context.Request.Cookies["access_token"];
            string cookieRefreshToken = context.Request.Cookies["refresh_token"];

            AuthenticationTicket requestTicket = GetTicket(context, requestAccessToken);
            AuthenticationTicket cookieTicket = GetTicket(context, cookieAccessToken);

            if (!object.Equals(requestTicket, null) && !IsExpired(requestTicket))
            {
                if (object.Equals(cookieTicket, null) || !string.Equals(requestTicket.Identity.Name, cookieTicket.Identity.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (await SignIn(context, requestAccessToken) == SignInStatus.Ok)
                        return;
                }
            }

            Guid refreshToken;
            if (Guid.TryParse(requestRefreshToken, out refreshToken))
            {
                if (await SignIn(context, requestRefreshToken) == SignInStatus.Ok)
                    return;
            }

            if (object.Equals(cookieTicket, null))
                return;

            context.Token = cookieAccessToken;

            if (IsExpired(cookieTicket))
                await RefreshToken(context, cookieRefreshToken);
        }

        private AuthenticationTicket GetTicket(OAuthRequestTokenContext context, string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return null;

            var tokenContext = new OAuthRequestTokenContext(context.OwinContext, accessToken);
            var tokenReceiveContext = new AuthenticationTokenReceiveContext(context.OwinContext, Startup.AuthenticationOptions.AccessTokenFormat, tokenContext.Token);
            tokenReceiveContext.DeserializeTicket(tokenReceiveContext.Token);

            return tokenReceiveContext.Ticket;
        }

        private bool IsExpired(AuthenticationTicket ticket)
        {
            if (object.Equals(ticket, null))
                return true;

            return ticket.Properties.ExpiresUtc.HasValue && ticket.Properties.ExpiresUtc.Value < Startup.AuthenticationOptions.SystemClock.UtcNow;
        }

        private async Task RefreshToken(OAuthRequestTokenContext context, string refreshToken)
        {
            try
            {
                var model = await _oauthService.RefreshTokenAsync(refreshToken);
                if (model.Status == RefreshTokenStatus.Ok)
                    UpdateToken(context, model.Token);
                else
                    RemoveToken(context);
            }
            catch
            {
                RemoveToken(context);
            }
        }
        private async Task<SignInStatus> SignIn(OAuthRequestTokenContext context, string token)
        {
            try
            {
                var model = await _oauthService.SignInAsync(token);
                if (model.Status == SignInStatus.Ok)
                    UpdateToken(context, model.Token);

                return model.Status;
            }
            catch { return SignInStatus.InvalidGrant; }
        }

        internal static void UpdateToken(OAuthRequestTokenContext context, SaaSApiToken token)
        {
            context.Response.Cookies.Append("access_token", token.AccessToken, _cookieOptions);
            context.Response.Cookies.Append("refresh_token", token.RefreshToken, _cookieOptions);
            context.Response.Cookies.Append("oauth_fullName", string.Format("{0} {1}", token.FirstName, token.LastName), _cookieOptions);

            context.Token = token.AccessToken;
        }
        private void RemoveToken(OAuthRequestTokenContext context)
        {
            context.Response.Cookies.Delete("access_token");
            context.Response.Cookies.Delete("refresh_token");
            context.Response.Cookies.Delete("oauth_fullName");

            context.Token = null;
        }

        public Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            return Task.FromResult<object>(null);
        }
    }
}