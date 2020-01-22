using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SaaS.IPDetect
{
    internal static class OwinContextExtensions
    {
        static readonly HashSet<string> _deniedIpAddress = null;

        static OwinContextExtensions()
        {
            var ipAddressFiltering = ConfigurationManager.GetSection("saas.ip.configuration") as IpAddressFilteringSection;
            if (!object.Equals(ipAddressFiltering, null))
            {
                _deniedIpAddress = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

                foreach (var entity in ipAddressFiltering.IpAddresses
                                                        .Cast<IpAddressElement>()
                                                        .Where(entity => entity.Denied))
                {
                    _deniedIpAddress.Add(entity.Address);
                }
            }
        }

        internal static bool IsIpAddressAllowed(this IOwinContext context)
        {
            if (object.Equals(_deniedIpAddress, null))
                return true;

            return !_deniedIpAddress.Contains(IpAddressDetector.IpAddress);
        }
    }
}