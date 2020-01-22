using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace SaaS.Common.Extensions
{
    public static class ExtensionMethods
    {
        public static string GetHash(this string input)
        {
            var hashAlgorithm = new SHA256CryptoServiceProvider();
            var byteValue = Encoding.UTF8.GetBytes(input);
            var byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }

        public static Uri CreateUri(this HttpRequestMessage request,string localPath)
        {
            var uri = new Uri(request.RequestUri.GetLeftPart(UriPartial.Authority));

            return new Uri(uri, localPath); 
        }
    }
}
