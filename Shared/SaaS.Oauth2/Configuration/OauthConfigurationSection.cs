using System.Configuration;

namespace SaaS.Oauth2.Configuration
{
    public class OauthConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("oauth", IsKey = false, IsRequired = true)]
        [ConfigurationCollection(typeof(OauthConfigurationElementCollection), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
        public OauthConfigurationElementCollection OAuthVClientConfigurations
        {
            get { return base["oauth"] as OauthConfigurationElementCollection; }
        }
    }
}
