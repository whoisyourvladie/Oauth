using System.Configuration;

namespace SaaS.Oauth2.Configuration
{
    public class OauthConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new OauthConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var authConfigurationElement = element as OauthConfigurationElement;
            if (authConfigurationElement != null) return authConfigurationElement.Name;
            return "";
        }
    }
}
