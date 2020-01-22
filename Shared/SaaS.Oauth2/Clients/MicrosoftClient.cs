using SaaS.Oauth2.Configuration;
using SaaS.Oauth2.Core;

namespace SaaS.Oauth2.Clients
{
    public class MicrosoftClient : ClientProvider
    {
        public MicrosoftClient()
        {
        }

        public MicrosoftClient(OauthConfigurationElement oauth)
            : base(oauth)
        {
        }
    }
}
