using SaaS.Common.Extensions;
using SaaS.Data.Entities;
using SaaS.Mailer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Mailer
{
    public class EmailRepository
    {
        private async Task EmailInsert<T>(Guid accountId, EmailTemplate templateId, string email, T model) where T : new()
        {
            var action = new SendEmailAction()
            {
                TemplateId = templateId,
                EmailToList = new List<XmlEmail>() { new XmlEmail { Email = email } }
            };

            string actionXml = XMLSerializer.SerializeObject(action);
            string modelXml = XMLSerializer.SerializeObject(model);

            using (var respository = new SaaS.Identity.EmailRepository())
                await respository.EmailInsertAsync(accountId, Status.NotStarted, actionXml, modelXml);
        }
        public async Task SendAccountCreationCompleteNotification(RecoverPasswordNotification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.AccountCreationCompleteNotification, model.Email, model);
        }
        public async Task SendBusinessDownloadNewAccountNotification(Notification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.BusinessDownloadNewAccountNotification, model.Email, model);
        }
        public async Task SendBusinessDownloadNotification(Notification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.BusinessDownloadNotification, model.Email, model);
        }
        public async Task SendRecoverPasswordNotification(RecoverPasswordNotification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.RecoverPasswordNotification, model.Email, model);
        }
        public async Task SendPasswordChangedNotification(Notification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.PasswordChangedNotification, model.Email, model);
        }
        public async Task SendEmailConfirmationCovermountNotification(EmailConfirmationNotification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.EmailConfirmationCovermountNotification, model.Email, model);
        }
        public async Task SendEmailConfirmationNotification(EmailConfirmationNotification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.EmailConfirmationNotification, model.Email, model);
        }
        public async Task SendEmailChangeConfirmationNotification(EmailChangeConfirmationNotification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.EmailChangeConfirmationNotification, model.NewEmail, model);
        }
        public async Task SendEmailChangeNotification(EmailChangeNotification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.EmailChangeNotification, model.Email, model);
        }
        public async Task SendMergeConfirmationNotification(MergeConfirmationNotification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.MergeConfirmationNotification, model.Email, model);
        }

        public async Task SendProductAssignedNotification(ProductAssignedNotification model)
        {
            var emailTemplate = EmailTemplate.ProductAssignedNotification;

            if (!string.IsNullOrEmpty(model.CreatePasswordLink))
                emailTemplate = EmailTemplate.ProductAssignedNotificationCreatePassword;

            await EmailInsert(model.AccountId, emailTemplate, model.Email, model);
        }
        public async Task SendProductEditionAssignedNotification(ProductAssignedNotification model)
        {
            var emailTemplate = EmailTemplate.ProductEditionAssignedNotification;

            if (!string.IsNullOrEmpty(model.CreatePasswordLink))
                emailTemplate = EmailTemplate.ProductEditionAssignedNotificationCreatePassword;

            await EmailInsert(model.AccountId, emailTemplate, model.Email, model);
        }
        public async Task SendProductSuspendNotification(ProductSuspendNotification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.ProductSuspendNotification, model.Email, model);
        }
        public async Task SendProductUnassignedNotification(ProductAssignedNotification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.ProductUnassignedNotification, model.Email, model);
        }

        public async Task SendLegacyActivationCreatePasswordNotification(CreatePasswordNotification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.LegacyActivationCreatePasswordNotification, model.Email, model);
        }

        public async Task SendLegacyActivationSignInNotification(Notification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.LegacyActivationSignInNotification, model.Email, model);
        }

        public async Task SendLegacyCreatePasswordReminder(CreatePasswordNotification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.LegacyCreatePasswordReminder, model.Email, model);
        }


        public async Task SendWelcomeNotification(Notification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.WelcomeNotification, model.Email, model);
        }

        public async Task MicrotransactionCreatePassword(CreatePasswordNotification model)
        {
            await EmailInsert(model.AccountId, EmailTemplate.MicrotransactionCreatePasswordNotification, model.Email, model);
        }
        
    }
}