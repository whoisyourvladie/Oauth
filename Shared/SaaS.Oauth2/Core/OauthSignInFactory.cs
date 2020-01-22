using SaaS.Oauth2.Configuration;
using System;
using System.Configuration;

namespace SaaS.Oauth2.Core
{
    internal static class OauthSignInFactory
    {
        internal static T CreateClient<T>(string configName) where T : ClientProvider, new()
        {
            var root = ConfigurationManager.GetSection("saas.oauth2.configuration") as OauthConfigurationSection;

            if (object.Equals(root, null))
                return default(T);

            var configurationReader = root.OAuthVClientConfigurations.GetEnumerator();

            while (configurationReader.MoveNext())
            {
                var currentOauthElement = configurationReader.Current as OauthConfigurationElement;
                if (currentOauthElement.Name == configName && currentOauthElement != null)
                    return (T)Activator.CreateInstance(typeof(T), new object[] { currentOauthElement });
            }

            throw new Exception("ERROR: [MultiOAuthFactroy] ConfigurationName is not found!");
        }
    }
}
