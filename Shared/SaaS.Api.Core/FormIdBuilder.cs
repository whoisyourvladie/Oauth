using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SaaS.Api.Core
{
    public static class FormIdBuilder
    {
        private static readonly Regex _hostRegex = new Regex(@"^www[0-9]*\.", _regexOptions);
        private static readonly RegexOptions _regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline;

        public static string Build(string formId, string suffix = null)
        {
            if (string.IsNullOrEmpty(formId))
                return null;

            var builder = new StringBuilder(formId.Replace("_", "-"));

            if (!string.IsNullOrEmpty(suffix))
                builder.Append(suffix);

            var urlReferrer = HttpContext.Current?.Request?.UrlReferrer;
            if (!object.Equals(urlReferrer, null))
            {
                var host = _hostRegex.Replace(urlReferrer.Host, string.Empty);
                var localPath = urlReferrer.LocalPath
                            .TrimEnd('/')
                            .Replace("/", ".");

                builder.AppendFormat("-{0}{1}", host, localPath);
            }

            return builder.ToString();
        }
    }
}
