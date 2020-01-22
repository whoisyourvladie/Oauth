using System;
using System.Linq;
using System.Text;
using System.Web;

namespace SaaS.Oauth2.Core
{
    internal static class QueryStringBuilder
    {
        internal static string BuildCompex(string[] dontEncode, params object[] keyValueEntries)
        {
            if (keyValueEntries.Length % 2 != 0)
            {
                throw new Exception(
                    "KeyAndValue collection needs to be dividable by two... key, value, key, value... get it?");
            }

            var sb = new StringBuilder();
            for (int index = 0; index < keyValueEntries.Length; index ++)
            {
                var key = keyValueEntries[index++];
                var value = keyValueEntries[index];

                if (object.Equals(value, null))
                    continue;

                var valEncoded = HttpUtility.UrlEncode(value.ToString());
                if (dontEncode != null && dontEncode.Contains(key))
                    valEncoded = value.ToString();

                sb.AppendFormat("{0}={1}&", key, valEncoded);
            }

            return sb.ToString().TrimEnd('&');
        }
    }
}
