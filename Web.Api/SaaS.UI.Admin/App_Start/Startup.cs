using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using SaaS.UI.Admin.Oauth;
using StructureMap;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace SaaS.UI.Admin
{
    public partial class Startup
    {
        public static OAuthBearerAuthenticationOptions AuthenticationOptions;

        static Startup()
        {
            AuthenticationOptions = new OAuthBearerAuthenticationOptions()
            {
                Provider = ObjectFactory.GetInstance<OauthBearerProvider>()
            };
        }


        public void ConfigureOAuth(IAppBuilder app)
        {
            app.UseOAuthBearerAuthentication(AuthenticationOptions);

            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();

            config.Services.Replace(typeof(IHttpControllerActivator), new ApiControllerActivator(config));

            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            //json.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            json.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
            json.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            json.SerializerSettings.DateFormatString = "yyyy-MM-ddThh:mm:sszzz";
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var contractResolver = (DefaultContractResolver)json.SerializerSettings.ContractResolver;
            contractResolver.IgnoreSerializableAttribute = true;
        }
    }

    public class ApiControllerActivator : IHttpControllerActivator
    {
        public ApiControllerActivator(HttpConfiguration configuration) { }

        public IHttpController Create(HttpRequestMessage request
            , HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = ObjectFactory.GetInstance(controllerType) as IHttpController;
            return controller;
        }
    }
}