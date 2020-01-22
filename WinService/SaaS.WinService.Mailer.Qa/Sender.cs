using SaaS.Identity;
using SaaS.Mailer;
using SaaS.Mailer.Models;
using System;
using System.Collections.Generic;

namespace SaaS.WinService.Mailer.Qa
{
    internal static class Sender
    {
        static IAuthRepository auth = new AuthRepository();

        internal static void AccountCreationCompleteNotification(string language, Notification notification)
        {
            notification = new RecoverPasswordNotification(notification, "https://sodapdf.com");
            Send(language, EmailTemplate.AccountCreationCompleteNotification, notification);
        }

        internal static void BusinessToPremiumNotification(string language, Notification notification)
        {
            Send(language, EmailTemplate.BusinessToPremiumNotification, notification);
        }
        internal static void eSignEmailConfirmationNotification(string language, Notification notification)
        {
            notification = new EmailConfirmationNotification(notification, "https://sodapdf.com/");
            Send(language, EmailTemplate.eSignEmailConfirmationNotification, notification);
        }
        internal static void eSignSignPackageNotification(string language, Notification notification)
        {
            Send(language, EmailTemplate.eSignSignPackageNotification, notification);
        }

        internal static void MicrotransactionCreatePassword(string language, Notification notification)
        {
            notification = new WelcomePurchaseNotification(notification, string.Empty) { CreatePasswordLink = "https://sodapdf.com/", };
            Send(language, EmailTemplate.MicrotransactionCreatePasswordNotification, notification);
        }
        internal static void MergeConfirmation(string language, Notification notification)
        {
            notification = new MergeConfirmationNotification(notification, "https://sodapdf.com/");
            Send(language, EmailTemplate.MergeConfirmationNotification, notification);
        }
        internal static void PasswordChangedNotification(string language, Notification notification)
        {
            Send(language, EmailTemplate.PasswordChangedNotification, notification);
        }
        internal static void ProductAssignedNotificationCreatePassword(string language, Notification notification)
        {
            Send(language, EmailTemplate.ProductAssignedNotificationCreatePassword, notification);
        }
        internal static void Welcome(string language, Notification notification)
        {
            Send(language, EmailTemplate.WelcomeNotification, notification);
        }
        internal static void WelcomePurchase(string language, Notification notification, string product)
        {
            notification = new WelcomePurchaseNotification(notification, product) { CreatePasswordLink = "https://sodapdf.com/" };
            var templateId = EmailTemplate.WelcomeFreeProductNotification;

            if (product == "Soda PDF Free")
                notification = new WelcomeFreeProductNotification(notification, product);

            switch (product)
            {
                case "Soda PDF Basic Plan": templateId = EmailTemplate.WelcomePurchaseBasicPlanNotification; break;
                case "Soda PDF Business Plan": templateId = EmailTemplate.WelcomePurchaseBusinessPlanNotification; break;

                case "Soda PDF Enterprise": templateId = EmailTemplate.WelcomePurchaseEnterpriseNotification; break;
                case "Soda PDF Home Edition": templateId = EmailTemplate.WelcomePurchaseHomeEditionNotification; break;
                case "Soda PDF Home Plan": templateId = EmailTemplate.WelcomePurchaseHomePlanNotification; break;
                case "Soda PDF Premium Edition": templateId = EmailTemplate.WelcomePurchasePremiumEditionNotification; break;
                case "Soda PDF Premium Plan": templateId = EmailTemplate.WelcomePurchasePremiumPlanNotification; break;
                case "Soda PDF Pro Edition": templateId = EmailTemplate.WelcomePurchaseProEditionNotification; break;
            }

#if PdfForge
            templateId = EmailTemplate.WelcomePurchaseNotification;
#endif

#if PdfSam
            templateId = EmailTemplate.WelcomePurchaseNotification;
#endif

            Send(language, templateId, notification);
        }


        internal static void WelcomeFreeProductCovermountNotification(string language, Notification notification, string product)
        {
            notification = new WelcomeFreeProductNotification(notification, product);
            Send(language, EmailTemplate.WelcomeFreeProductCovermountNotification, notification);
        }
        internal static void WelcomePurchaseMigrationFromSuiteNotification(string language, Notification notification, string product)
        {
            notification = new WelcomePurchaseNotification(notification, product);
            (notification as WelcomePurchaseNotification).CreatePasswordLink = "link";
            Send(language, EmailTemplate.WelcomePurchaseMigrationFromSuiteNotification, notification);
        }        

        internal static void EmailConfirmationCovermount(string language, Notification notification)
        {
            notification = new EmailConfirmationNotification(notification, "https://sodapdf.com/");
            Send(language, EmailTemplate.EmailConfirmationCovermountNotification, notification);
        }
        internal static void EmailConfirmation(string language, Notification notification)
        {
            notification = new EmailConfirmationNotification(notification, "https://sodapdf.com/");
            Send(language, EmailTemplate.EmailConfirmationNotification, notification);
        }
        internal static void EmailConfirmationLate(string language, Notification notification)
        {
            notification = new EmailConfirmationNotification(notification, "https://sodapdf.com/");
            Send(language, EmailTemplate.EmailConfirmationLateNotification, notification);
        }

        internal static void EmailChange(string language, Notification notification)
        {
            notification = new EmailChangeNotification(notification, string.Empty);
            Send(language, EmailTemplate.EmailChangeNotification, notification);
        }
        internal static void EmailChangeConfirmation(string language, Notification notification)
        {
            notification = new EmailChangeConfirmationNotification(notification, string.Empty, "https://sodapdf.com/");
            Send(language, EmailTemplate.EmailChangeConfirmationNotification, notification);
        }

        internal static void OsSunset(string language, Notification notification)
        {
            Send(language, EmailTemplate.OsSunsetNotification, notification);
        }
        internal static void PolicyUpdate(string language, Notification notification)
        {
            Send(language, EmailTemplate.PolicyUpdateNotification, notification);
        }
        internal static void ProductSuspend(string language, Notification notification, string product)
        {
            notification = new ProductSuspendNotification(notification, product, DateTime.Now);
            Send(language, EmailTemplate.ProductSuspendNotification, notification);
        }
        internal static void ProductRenewalOff(string language, Notification notification, string product)
        {
            notification = new ProductRenewalOffNotification(notification, product);
            Send(language, EmailTemplate.ProductRenewalOffNotification, notification);
        }
        internal static void ProductAssignedCreatePassword(string language, Notification notification, string product)
        {
            notification = new ProductAssignedNotification(notification, product, notification) { DownloadLink = "https://sodapdf.com/" };
            Send(language, EmailTemplate.ProductAssignedNotificationCreatePassword, notification);
        }
        internal static void ProductUnassigned(string language, Notification notification, string product)
        {
            notification = new ProductAssignedNotification(notification, product, notification);
            Send(language, EmailTemplate.ProductUnassignedNotification, notification);
        }

        private static void Send(string language, EmailTemplate templateId, Notification notification)
        {
            var emailAction = new SendEmailAction { EmailToList = new List<XmlEmail>(), TemplateId = templateId };
            emailAction.EmailToList.Add(new XmlEmail { Email = notification.Email });

            var parser = new RazorParser(language);
            string subject = parser.ParseSubject(emailAction, notification);
            string body = parser.ParseTemplate(emailAction, notification, subject);

            EmailManager.Send(emailAction, subject, body);
        }        
    }
}
