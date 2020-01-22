using SaaS.Oauth2.Configuration;
using SaaS.Oauth2.Core;

namespace SaaS.Oauth2.Clients
{
    public class FacebookClient : ClientProvider
    {
        public FacebookClient()
        {
        }

        public FacebookClient(OauthConfigurationElement oauth)
            : base(oauth)
        {
        }
    }
}
