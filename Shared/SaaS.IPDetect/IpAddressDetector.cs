using System;
using System.Configuration;
using System.Net;
using System.Web;

namespace SaaS.IPDetect
{
    public static class IpAddressDetector
    {
        private static char[] _splitCharacters = new char[] { ',' };
        private static string _ipAddressServerVariableName = ConfigurationManager.AppSettings["IpAddressServerVariableName"];

        public static string IpAddress
        {
            get
            {
                string ipAddress = null;
                var request = HttpContext.Current.Request;

                if (!string.IsNullOrWhiteSpace(_ipAddressServerVariableName))
                    ipAddress = request.ServerVariables[_ipAddressServerVariableName];

                if (string.IsNullOrWhiteSpace(ipAddress))
                    ipAddress = request.UserHostAddress;

                if (!string.IsNullOrWhiteSpace(ipAddress) && ipAddress.IndexOf(',') != -1)
                {
                    var split = ipAddress.Split(_splitCharacters, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in split)
                    {
                        var value = (item ?? string.Empty).Trim();

                        IPAddress tmpIpAddress;
                        if (IPAddress.TryParse(value, out tmpIpAddress))
                            return value;
                    }
                }
                return ipAddress;
            }
        }
    }
}