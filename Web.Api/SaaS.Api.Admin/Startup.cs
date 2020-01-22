using AutoMapper;
using Microsoft.Owin;
using Owin;
using SaaS.Api.Models.Oauth;
using SaaS.Api.Models.Products;
using SaaS.Data.Entities.View;
using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Web.Http;

[assembly: OwinStartup(typeof(SaaS.Api.Admin.Startup))]
namespace SaaS.Api.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            ConfigureOAuth(app);

            var config = new HttpConfiguration();
            WebApiConfig.Register(config);

            app.Map("/api", inner =>
            {
                inner.UseWebApi(config);
            });



            Mapper.Initialize(cfg => {

                cfg.CreateMap<ViewAccountProduct, AccountProductViewModel>();
                cfg.CreateMap<ViewAccountProductModule, AccountProductModuleModel>();

                cfg.CreateMap<ViewAccountDetails, AccountDetailsViewModel>();
                cfg.CreateMap<AccountDetailsViewModel, ViewAccountDetails>();
                cfg.CreateMap<RegisterViewModel, ViewAccountDetails>();

                cfg.CreateMap<ViewUpgradeProduct, UpgradeProductViewModel>();

                cfg.CreateMap<ViewOwnerProduct, OwnerProductViewModel>();
            });
        }
    }

    public sealed class AppSettings : IAppSettings
    {
        public AppSettingsOauth Oauth { get; private set; }
        public AppSettingsUpclick Upclick { get; private set; }

        public Uri DownloadLink { get; private set; }

        public AppSettings()
        {
            Oauth = new AppSettingsOauth(Setting<string>("oauth:resetPassword"));
            Upclick = new AppSettingsUpclick(Setting<string>("upclick:merchantLogin"), Setting<string>("upclick:merchantPassword"));

            DownloadLink = new Uri(Setting<string>("downloadLink"));
        }

        private static T Setting<T>(string name)
        {
            string value = ConfigurationManager.AppSettings[name];

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }

    public class AppSettingsUpclick
    {
        public readonly string MerchantLogin;
        public readonly string MerchantPassword;

        public AppSettingsUpclick(string merchantLogin, string merchantPassword)
        {
            MerchantLogin = merchantLogin;
            MerchantPassword = merchantPassword;
        }
    }

    public interface IAppSettings
    {
        Uri DownloadLink { get; }

        AppSettingsOauth Oauth { get; }
        AppSettingsUpclick Upclick { get; }
    }

    public class AppSettingsOauth
    {
        public AppSettingsOauth(string resetPassword)
        {
            ResetPassword = new Uri(resetPassword);
        }

        public readonly Uri ResetPassword;
    }
}