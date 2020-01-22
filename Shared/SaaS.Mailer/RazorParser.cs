using RazorEngine;
using RazorEngine.Templating;
using SaaS.Common.Extensions;
using SaaS.Mailer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Web;

namespace SaaS.Mailer
{
    public class RazorParser
    {
        private static readonly string _templateFolder;
        private static readonly ResourceManager _resourceManager = null;
        private static readonly IAppSettings _appSettigns = new AppSettings();
        private static readonly Dictionary<string, CultureInfo> _cultures = new Dictionary<string, CultureInfo>(StringComparer.InvariantCultureIgnoreCase);

        static RazorParser()
        {
            _cultures.Add("en", new CultureInfo("en-US"));
            _cultures.Add("fr", new CultureInfo("fr-FR"));
            _cultures.Add("de", new CultureInfo("de-DE"));
            _cultures.Add("it", new CultureInfo("it-IT"));
            _cultures.Add("es", new CultureInfo("es-ES"));
            _cultures.Add("pt", new CultureInfo("pt-PT"));
            _cultures.Add("ru", new CultureInfo("ru-RU"));
            _cultures.Add("jp", new CultureInfo("ja-JP"));
            _cultures.Add("ja", new CultureInfo("ja-JP"));
            _cultures.Add("sv", new CultureInfo("sv-FI"));

            var brand = string.Empty;

#if LuluSoft
            brand = "LuluSoft";
            _resourceManager = Templates.LuluSoft.App_GlobalResources.Subjects.ResourceManager;
#endif

#if PdfForge
            brand = "PdfForge";
            _resourceManager = Templates.PdfForge.App_GlobalResources.Subjects.ResourceManager;
#endif

#if PdfSam
            brand = "PdfSam";
            _resourceManager = Templates.PdfSam.App_GlobalResources.Subjects.ResourceManager;
#endif

            _templateFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates", brand);

#if LuluSoft && DEBUG
            _templateFolder = @"d:\svn\Canada\oauth.sodapdf.com\branches\spdf-10971(sprint-67)\Shared\SaaS.Mailer\Templates\LuluSoft";
#endif

#if PdfForge && DEBUG
            _templateFolder = @"C:\projects\svn\oauth.sodapdf.com\branches\pa-2566(no-sprint)\Shared\SaaS.Mailer\Templates\PdfForge";
#endif

#if PdfSam && DEBUG
            _templateFolder = @"d:\svn\Canada\oauth.sodapdf.com\branches\spdf-10971(sprint-67)\Shared\SaaS.Mailer\Templates\PdfSam\";
#endif

        }
        public RazorParser(string lang)
        {
            lang = lang ?? string.Empty;
            Thread.CurrentThread.CurrentUICulture = _cultures.ContainsKey(lang) ? _cultures[lang] : _cultures.First().Value;
        }

        public string ParseTemplateAsync(SendEmailAction emailAction, string xml, out object model)
        {
            model = null;
            switch (emailAction.TemplateId)
            {
                case EmailTemplate.EmailChangeConfirmationNotification:
                    model = XMLSerializer.DeserializeObject<EmailChangeConfirmationNotification>(xml); break;

                case EmailTemplate.eSignEmailConfirmationNotification:
                case EmailTemplate.EmailConfirmationLateNotification:

                    var emailConfirmation = XMLSerializer.DeserializeObject<EmailConfirmationNotification>(xml);
                    var emailConfirmationUriBuilder = new UriBuilder(_appSettigns.Oauth.EmailConfirmation);
                    var emailConfirmationQuery = HttpUtility.ParseQueryString(emailConfirmationUriBuilder.Query);
                    emailConfirmationQuery.Add("userId", emailConfirmation.AccountId.ToString("N"));
                    emailConfirmationUriBuilder.Query = emailConfirmationQuery.ToString();

                    emailConfirmation.ConfirmationLink = emailConfirmationUriBuilder.Uri.ToString();

                    model = emailConfirmation;

                    break;

                case EmailTemplate.EmailConfirmationCovermountNotification:
                case EmailTemplate.EmailConfirmationNotification: model = XMLSerializer.DeserializeObject<EmailConfirmationNotification>(xml); break;

                case EmailTemplate.EmailChangeNotification: model = XMLSerializer.DeserializeObject<EmailChangeNotification>(xml); break;

                case EmailTemplate.ProductSuspendNotification: model = XMLSerializer.DeserializeObject<ProductSuspendNotification>(xml); break;

                case EmailTemplate.AccountCreationCompleteNotification:
                case EmailTemplate.RecoverPasswordNotification: model = XMLSerializer.DeserializeObject<RecoverPasswordNotification>(xml); break;

                case EmailTemplate.ProductAssignedNotification:
                case EmailTemplate.ProductAssignedNotificationCreatePassword:
                case EmailTemplate.ProductEditionAssignedNotification:
                case EmailTemplate.ProductEditionAssignedNotificationCreatePassword:
                case EmailTemplate.ProductUnassignedNotification: model = XMLSerializer.DeserializeObject<ProductAssignedNotification>(xml); break;
                case EmailTemplate.ProductRenewalOffNotification: model = XMLSerializer.DeserializeObject<ProductRenewalOffNotification>(xml); break;

                case EmailTemplate.WelcomeFreeProductNotification:
                case EmailTemplate.WelcomeFreeProductCovermountNotification:
                    model = XMLSerializer.DeserializeObject<WelcomeFreeProductNotification>(xml); break;

                case EmailTemplate.WelcomePurchaseNotification:
                case EmailTemplate.WelcomePurchaseHomePlanNotification:
                case EmailTemplate.WelcomePurchasePremiumPlanNotification:
                case EmailTemplate.WelcomePurchaseBasicPlanNotification:
                case EmailTemplate.WelcomePurchaseHomeEditionNotification:
                case EmailTemplate.WelcomePurchaseProEditionNotification:
                case EmailTemplate.WelcomePurchasePremiumEditionNotification:
                case EmailTemplate.WelcomePurchaseEnterpriseNotification:
                case EmailTemplate.MicrotransactionCreatePasswordNotification:
                case EmailTemplate.WelcomePurchaseProOcrEditionNotification: //pdfsam
                case EmailTemplate.WelcomePurchaseStandardEditionNotification: //pdfsam
                case EmailTemplate.WelcomePurchaseStandardPlanNotification: //pdfsam
                case EmailTemplate.WelcomePurchaseProPlanNotification: //pdfsam
                case EmailTemplate.WelcomePurchaseProOcrPlanNotification: //pdfsam
                case EmailTemplate.WelcomePurchaseOcrPlanNotification: //pdfsam
                case EmailTemplate.WelcomePurchaseEditPlanNotification: //pdfsam
                case EmailTemplate.WelcomePurchaseConvertPlanNotification: //pdfsam
                case EmailTemplate.WelcomePurchaseMigrationFromSuiteNotification: //pdfsuite to soda

                    var welcomeModel = XMLSerializer.DeserializeObject<WelcomePurchaseNotification>(xml);

                    var uriBuilder = new UriBuilder(_appSettigns.Oauth.CreatePassword);
                    if (emailAction.TemplateId == EmailTemplate.WelcomePurchaseMigrationFromSuiteNotification)
                        uriBuilder = new UriBuilder(new Uri(string.Format(_appSettigns.Oauth.CustomLink, "online-createpassword-suite-to-soda")));
                    var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                    query.Add("userId", welcomeModel.AccountId.ToString("N"));
                    query.Add("email", welcomeModel.Email);
                    query.Add("firstName", welcomeModel.FirstName);
                    query.Add("lastName", welcomeModel.LastName);

                    uriBuilder.Query = query.ToString();

                    welcomeModel.CreatePasswordLink = uriBuilder.Uri.ToString();

                    model = welcomeModel;

                    break;

                case EmailTemplate.LegacyCreatePasswordReminder:
                case EmailTemplate.LegacyActivationCreatePasswordNotification:
                    model = XMLSerializer.DeserializeObject<CreatePasswordNotification>(xml); break;

                case EmailTemplate.MergeConfirmationNotification:
                    model = XMLSerializer.DeserializeObject<MergeConfirmationNotification>(xml); break;

                case EmailTemplate.eSignSignPackageNotification: model = XMLSerializer.DeserializeObject<XmlNotification>(xml); break;

                default: model = XMLSerializer.DeserializeObject<Notification>(xml); break;
            }

            var subject = ParseSubject(emailAction, model);
            return ParseTemplate(emailAction, model, subject);
        }
        public string ParseTemplate(SendEmailAction emailAction, object model, string subject)
        {
            string layoutPath = Path.Combine(_templateFolder, "Partial\\Layout.cshtml");
            string partialPath = Path.Combine(_templateFolder, string.Format("{0}.cshtml", emailAction.TemplateId));

            string layoutHtml = Parse(layoutPath, model);
            string partialHtml = Parse(partialPath, model);

            var builder = new StringBuilder(layoutHtml);
            builder.Replace("**partial**", partialHtml);
            builder.Replace("**subject**", subject);

            return builder.ToString();
        }
        public string ParseSubject(SendEmailAction emailAction, object model)
        {
            string subject = _resourceManager.GetString(emailAction.TemplateId.ToString());

            if (!string.IsNullOrEmpty(subject))
            {
                var type = model.GetType();
                var productName = type.GetProperty("ProductName", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (!object.Equals(productName, null))
                {
                    var value = productName.GetValue(model, null) as string;

                    subject = subject.Replace("[product name]", value);
                    subject = subject.Replace("[Product Name]", value); //say hello for Ksenia
                }
            }

            return string.IsNullOrEmpty(subject) ? "SaaS" : subject;
        }

        private string Parse<TModel>(string templatePath, TModel model)
        {
            if (!Engine.Razor.IsTemplateCached(templatePath, typeof(TModel)))
                Engine.Razor.AddTemplate(templatePath, File.ReadAllText(templatePath));

            return Engine.Razor.RunCompile(templatePath, typeof(TModel), model);
        }
    }
}
