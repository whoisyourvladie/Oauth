using SaaS.Oauth2.Core;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Oauth2.Services
{
    public abstract class OauthService
    {
        protected ClientProvider _clientProvider;

        protected OauthService(ClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public abstract string GetAuthenticationUrl(string lang);
        public abstract Size GetWindowSize();

        public abstract Task<TokenResult> TokenAsync(string code, CancellationToken cancellationToken);
        public abstract Task<ProfileResult> ProfileAsync(TokenResult token, CancellationToken cancellationToken);
    }
}