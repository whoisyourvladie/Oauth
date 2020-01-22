using NLog;
using SaaS.Data.Entities.Accounts;
using SaaS.Data.Entities.View;
using System.Net.Http;
using System.Text;

namespace SaaS.Api.Oauth
{
    internal static class OauthLogger
    {
        //private static Logger _oauthCreateAccountLogger;

        static OauthLogger()
        {
            //_oauthCreateAccountLogger = LogManager.GetLogger("oauth-create-account");
            //_oauthCreateAccountLogger.Warn("Start");
        }

        internal static void CreateAccountWarn(HttpRequestMessage request, Account account, ViewAccountDetails details, string stackTrace)
        {
            //string message = null;

            //if (object.Equals(details, null))
            //    message = "ViewAccountDetails is null.";

            //if (!object.Equals(details, null) && !details.Uid.HasValue)
            //    message = "ViewAccountDetails.Uid is null.";

            //if (!object.Equals(message, null))
            //{
            //    var referrer = string.Empty;
            //    if (!object.Equals(request.Headers, null) && !object.Equals(request.Headers.Referrer, null))
            //        referrer = request.Headers.Referrer.ToString();

            //    var source = string.Empty;
            //    if (!object.Equals(details, null))
            //        source = details.Source;

            //    var builder = new StringBuilder();
            //    builder.AppendLine(message);
            //    builder.AppendLine(string.Format("AccountId: {0}", account.Id));
            //    builder.AppendLine(string.Format("Referrer: {0}", referrer));
            //    builder.AppendLine(string.Format("Stack trace: {0}", stackTrace));
            //    builder.AppendLine(string.Format("Source: {0}", source));

            //    _oauthCreateAccountLogger.Warn(builder.ToString());
            //}
        }
    }
}