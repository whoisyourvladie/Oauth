using SaaS.Oauth2.Configuration;

namespace SaaS.Oauth2.Core
{
    public abstract class ClientProvider
    {
        protected ClientProvider() { }

        protected ClientProvider(OauthConfigurationElement oauth)
        {
            ClientId = oauth.ClientId;
            ClientSecret = oauth.ClientSecret;
            CallBackUrl = oauth.CallbackUrl;
            Scope = oauth.Scope;
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CallBackUrl { get; set; }
        public string Scope { get; set; }

        public string AccessToken { get; set; }
        public string TokenSecret { get; set; }
    }
}
