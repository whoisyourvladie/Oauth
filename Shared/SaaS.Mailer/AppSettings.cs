using System;
using System.Configuration;
using System.Globalization;

namespace SaaS.Mailer
{
    public sealed class AppSettings : IAppSettings
    {
        public AppSettingsOauth Oauth { get; private set; }

        public AppSettings()
        {
            Oauth = new AppSettingsOauth(Setting<string>("oauth:createPassword"), Setting<string>("oauth:emailConfirmation"), Setting<string>("paygw:customLink"));
        }

        private static T Setting<T>(string name)
        {
            string value = ConfigurationManager.AppSettings[name];

            if (value == null)
                throw new Exception(string.Format("Could not find setting '{0}',", name));

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }

    public interface IAppSettings
    {
        AppSettingsOauth Oauth { get; }
    }

    public class AppSettingsOauth
    {
        public AppSettingsOauth(string createPassword, string emailConfirmation, string customLink)
        {
            CreatePassword = new Uri(createPassword);
            EmailConfirmation = new Uri(emailConfirmation);
            CustomLink = customLink;
        }

        public readonly Uri CreatePassword;
        public readonly Uri EmailConfirmation;
        public readonly string CustomLink;
    }
}
