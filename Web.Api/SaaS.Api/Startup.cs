using AutoMapper;
using Microsoft.Owin;
using Owin;
using SaaS.Api.Models.Oauth;
using SaaS.Api.Models.Products;
using SaaS.Data.Entities.View;
using SaaS.IPDetect;
using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Web.Http;

[assembly: OwinStartup(typeof(SaaS.Api.Startup))]
namespace SaaS.Api
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
                inner.Use(typeof(IpAddressFilterMiddleware));
                inner.UseWebApi(config);
            });

            Mapper.Initialize(cfg =>
            {

                cfg.CreateMap<ViewAccountProduct, AccountProductViewModel>();
                cfg.CreateMap<ViewAccountProductModule, AccountProductModuleModel>();

                cfg.CreateMap<ViewAccountDetails, AccountDetailsViewModel>();
                cfg.CreateMap<AccountDetailsViewModel, ViewAccountDetails>();
                cfg.CreateMap<RegisterViewModel, ViewAccountDetails>()
                    .ForMember(dest => dest.IsTrial, opts => opts.MapFrom(src => src.Trial));

                cfg.CreateMap<ViewUpgradeProduct, UpgradeProductViewModel>();

                cfg.CreateMap<ViewOwnerProduct, OwnerProductViewModel>();
            });
        }
    }

    public sealed class AppSettings : IAppSettings
    {
        public AppSettingsOauth Oauth { get; private set; }
        public AppSettingsUpclick Upclick { get; private set; }
        public AppSettingsB2BLead B2BLead { get; private set; }


        public Uri DownloadLink { get; private set; }

        public long ActivationAccountTimeSpan { get; private set; }

        public AppSettings()
        {
            Oauth = new AppSettingsOauth(Setting<string>("oauth:resetPassword"), Setting<string>("oauth:createPassword"),
                                         Setting<string>("oauth:emailConfirmation"),
                                         Setting<string>("oauth:mergeConfirmation"),
                                         Setting<string>("oauth:emailChangeConfirmation"));
            Upclick = new AppSettingsUpclick(Setting<string>("upclick:merchantLogin"), Setting<string>("upclick:merchantPassword"));
            B2BLead = new AppSettingsB2BLead(Setting<string>("b2blead:Email"));

            DownloadLink = new Uri(Setting<string>("downloadLink"));

            ActivationAccountTimeSpan = (long)TimeSpan.FromDays(1).TotalSeconds;
        }

        private static T Setting<T>(string name)
        {
            string value = ConfigurationManager.AppSettings[name];

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }

    public interface IAppSettings
    {
        AppSettingsOauth Oauth { get; }
        AppSettingsUpclick Upclick { get; }
        AppSettingsB2BLead B2BLead { get; }

        Uri DownloadLink { get; }

        long ActivationAccountTimeSpan { get; }
    }

    public class AppSettingsOauth
    {
        public AppSettingsOauth(string resetPassword, string createPassword,
            string emailConfirmation,
            string mergeConfirmation,
            string emailChangeConfirmation)
        {
            ResetPassword = new Uri(resetPassword);
            CreatePassword = new Uri(createPassword);
            EmailConfirmation = new Uri(emailConfirmation);
            MergeConfirmation = new Uri(mergeConfirmation);
            EmailChangeConfirmation = new Uri(emailChangeConfirmation);
        }

        public readonly Uri ResetPassword;
        public readonly Uri CreatePassword;
        public readonly Uri EmailConfirmation;
        public readonly Uri MergeConfirmation;
        public readonly Uri EmailChangeConfirmation;
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
    public class AppSettingsB2BLead
    {
        public readonly string Email;

        public AppSettingsB2BLead(string email)
        {
            Email = email;
        }
    }

    public sealed class AppSettingsValidation
    {
        public string FirstNamePattern { get; internal set; }
        public string LastNamePattern { get; internal set; }
        public string EmailPattern { get; internal set; }
    }
}