using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;

namespace SaaS.Api.Sign.Providers
{
    public class OauthBearerProvider : IOAuthBearerAuthenticationProvider
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