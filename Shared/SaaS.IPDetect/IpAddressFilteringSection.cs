using System.Configuration;

namespace SaaS.IPDetect
{
    public class IpAddressFilteringSection : ConfigurationSection
    {
        [ConfigurationProperty("ipAddresses", IsDefaultCollection = true)]
        public IpAddressElementCollection IpAddresses
        {
            get { return (IpAddressElementCollection)this["ipAddresses"]; }
            set { this["ipAddresses"] = value; }
        }
    }
}
