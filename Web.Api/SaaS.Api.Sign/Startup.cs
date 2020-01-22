using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using SaaS.Api.Sign.Providers;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

[assembly: OwinStartup(typeof(SaaS.Api.Sign.Startup))]
namespace SaaS.Api.Sign
{
    public partial class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions;

        static Startup()
        {
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions()
            {
                Provider = new OauthBearerProvider()
            };
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            var config = GlobalConfiguration.Configuration;

            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
            app.UseWebApi(config);

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();

            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            json.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            json.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            json.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;

            var contractResolver = (DefaultContractResolver)json.SerializerSettings.ContractResolver;
            contractResolver.IgnoreSerializableAttribute = true;
        }
    }
}