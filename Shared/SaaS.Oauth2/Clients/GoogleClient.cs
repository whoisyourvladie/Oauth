using SaaS.Oauth2.Configuration;
using SaaS.Oauth2.Core;

namespace SaaS.Oauth2.Clients
{
    public class GoogleClient : ClientProvider
    {
        public GoogleClient()
        {
        }

        public GoogleClient(OauthConfigurationElement oauth)
            : base(oauth)
        {
        }
    }
}
