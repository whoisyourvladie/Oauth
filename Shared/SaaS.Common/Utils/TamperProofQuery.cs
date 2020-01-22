using SaaS.Common.Extensions;
using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Linq;

namespace SaaS.Common.Utils
{
    public class TamperProofQuery
    {
        private readonly string _secret;
        private const string DefaultHashAlgorithm = "SHA256";

        public TamperProofQuery(string secret = "S@@S S0d@PDF")
        {
            _secret = secret;
        }

        public string ComputeDigest(string[] values, string hashAlgorithmName = DefaultHashAlgorithm)
        {
            if (string.IsNullOrEmpty(hashAlgorithmName))
                hashAlgorithmName = DefaultHashAlgorithm;

            string saltedValues = String.Concat(values) + _secret;

            return ComputeHash(Encoding.UTF8.GetBytes(saltedValues), hashAlgorithmName);
        }

        public string ComputeDigestQuery(NameValueCollection query, out string encoded)
        {
            query["timestamp"] = TimeHelper.GetEpochTime().ToString();

            string queryString = string.Join("&", query.AllKeys.Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(query[key]))));
            queryString = queryString.EncryptString();

            encoded = HttpUtility.UrlEncode(queryString);

            return ComputeDigest(new string[] { queryString });
        }

        public bool IsTampered(string digest, string[] values, string hashAlgorithmName = DefaultHashAlgorithm)
        {
            if (string.IsNullOrEmpty(hashAlgorithmName))
                hashAlgorithmName = DefaultHashAlgorithm;

            return !string.Equals(digest, ComputeDigest(values, hashAlgorithmName));
        }

        public bool IsExpired(long expirePeriod)
        {
            return TimeHelper.GetEpochTime() - expirePeriod > 0;
        }

        ///// <summary>
        ///// check if timestamp is valid
        ///// </summary>
        ///// <param name="timestamp">time stamp</param>
        ///// <param name="expirePeriod">expire period in seconds</param>
        ///// <returns></returns>
        //public bool IsExpired(string timestamp, long expirePeriod)
        //{
        //    long timestampInt;
        //    if (!long.TryParse(timestamp, out timestampInt))
        //        throw new ArgumentException("Time stamp is not valid integer number", "timestamp");
        //    return IsExpired(timestampInt, expirePeriod);
        //}

        //public bool IsExpired(long timestamp, long expirePeriod)
        //{
        //    return TimeHelper.GetEpochTime() - timestamp > expirePeriod;
        //}

        private string ComputeHash(byte[] content, string hashAlgorithmName)
        {
            byte[] hashByteArray;
            using (HashAlgorithm algorithm = HashAlgorithm.Create(hashAlgorithmName))
            {
                if (algorithm == null)
                    throw new ArgumentException("Unrecognized hash name", "hashAlgorithmName");

                hashByteArray = algorithm.ComputeHash(content);
            }
            return BitConverter.ToString(hashByteArray).Replace("-", "");
        }
    }
}
