namespace SaaS.UI.Admin.App_Start
{
    public interface IAppSettings
    {
        IAppSettingsOAuth OAuth { get; }
        IAppSettingsValidation Validation { get; }
    }
    public interface IAppSettingsOAuth
    {
        string Path { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string Scope { get; }
    }

    public interface IAppSettingsValidation
    {
        string FirstNamePattern { get; }
        string LastNamePattern { get; }
        string EmailPattern { get; }
        string UrlPattern { get; }
        string JsFirstNamePattern { get; }
        string JsLastNamePattern { get; }
        string JsEmailPattern { get; }
        string JsUrlPattern { get; }
    }
    public sealed class AppSettings : IAppSettings
    {
        public IAppSettingsOAuth OAuth { get; internal set; }
        public IAppSettingsValidation Validation { get; internal set; }
    }

    public sealed class AppSettingsOAuth : IAppSettingsOAuth
    {
        public string Path { get; internal set; }
        public string ClientId { get; internal set; }
        public string ClientSecret { get; internal set; }
        public string Scope { get; internal set; }
    }

    public sealed class AppSettingsValidation : IAppSettingsValidation
    {
        public string FirstNamePattern { get; internal set; }
        public string LastNamePattern { get; internal set; }
        public string EmailPattern { get; internal set; }
        public string UrlPattern { get; internal set; }

        public string JsFirstNamePattern
        {
            get { return string.Format("/{0}/", FirstNamePattern); }
        }
        public string JsLastNamePattern
        {
            get { return string.Format("/{0}/", LastNamePattern); }
        }
        public string JsEmailPattern
        {
            get { return string.Format("/{0}/i", EmailPattern); }
        }

        public string JsUrlPattern
        {
            get { return string.Format("/{0}/", UrlPattern); }
        }
    }
}