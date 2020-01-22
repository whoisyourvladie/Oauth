using System;
using System.Collections.Specialized;
using System.Threading;
using System.Web;

namespace SaaS.Mailer
{
    public static class links
    {
        private static string GetLink(string action, NameValueCollection collection = null, string fragment = null)
        {
            var uri = string.Empty;

#if LuluSoft
            uri = "http://paygw.sodapdf.com/redirect/{0}/soda-pdf-desktop/";
#endif

#if PdfForge
            uri = "http://paygw.pdfarchitect.org/redirect/{0}/pdf-architect-6/";
#endif

#if PdfSam
            uri = "https://paygw.pdfsam.org/redirect/{0}/pdfsam-enhanced-6/";
#endif

            var uriBuilder = new UriBuilder(string.Format(uri, action));
            var query = HttpUtility.ParseQueryString(string.Empty);

            query.Add("lang", Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2));

            if (!object.Equals(collection, null))
            {
                foreach (string item in collection)
                    query.Set(item, collection[item]);
            }

            uriBuilder.Query = query.ToString();
            uriBuilder.Fragment = fragment;

            return uriBuilder.Uri.ToString();
        }

        public static string Localize(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query.Add("lang", Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2));
            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri.ToString();
        }

        private static string GetCustomLink(string customValue, string fragment = null)
        {
            return GetLink("custom", new NameValueCollection { { "customValue", customValue } }, fragment);
        }

        public static string getTrial { get { return GetCustomLink("get-trial"); } }
        public static string saas { get { return GetCustomLink("saas"); } }
        public static string saasEsign { get { return GetCustomLink("saas", "esign"); } }
        public static string privacy { get { return GetLink("privacy"); } }
        public static string terms { get { return GetCustomLink("online-terms"); } }
        public static string subscribe { get { return GetCustomLink("subscribe"); } }
        public static string unsubscribe { get { return GetCustomLink("subscribe"); } }
        public static string support { get { return GetLink("support"); } }
        public static string upgrade { get { return GetLink("upgrade"); } }
        public static string productPricing { get { return GetCustomLink("product-and-pricing"); } }
        public static string account { get { return GetCustomLink("online-account"); } }
        public static string knowledgeBase { get { return GetCustomLink("knowledge-base"); } }
        public static string knowledgeBaseSwitchProduct { get { return GetCustomLink("knowledge-base-switch-product"); } }
        public static string knowledgeBaseCreatePassword { get { return GetCustomLink("knowledge-base-create-password"); } }
        public static string knowledgeBaseModifyPlan { get { return GetCustomLink("knowledge-base-modify-plan"); } }
    }
}