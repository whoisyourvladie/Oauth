using SaaS.Oauth2.Clients;
using SaaS.Oauth2.Core;
using System;

namespace SaaS.Oauth2.Services
{
    public static class OauthServiceFactory
    {
        public static OauthService CreateService(string provider)
        {
            var comparison = StringComparison.InvariantCultureIgnoreCase;

            if ("google".Equals(provider, comparison))
                return new GoogleService(OauthSignInFactory.CreateClient<GoogleClient>("Google"));

            if ("facebook".Equals(provider, comparison))
                return new FacebookService(OauthSignInFactory.CreateClient<FacebookClient>("Facebook"));

            if ("microsoft".Equals(provider, comparison))
                return new MicrosoftService(OauthSignInFactory.CreateClient<MicrosoftClient>("Microsoft"));

            return null;
        }
    }
}
