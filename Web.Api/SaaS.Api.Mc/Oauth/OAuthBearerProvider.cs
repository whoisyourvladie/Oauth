using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;

namespace SaaS.Api.Mc.Oauth
{
    public class OAuthBearerProvider : IOAuthBearerAuthenticationProvider
    {
        public Task ApplyChallenge(OAuthChallengeContext context)
        {
            return Task.FromResult<object>(null);
        }

        public Task RequestToken(OAuthRequestTokenContext context)
        {
            return Task.FromResult<object>(null);
        }
        public Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            return Task.FromResult<object>(null);
        }
    }
}