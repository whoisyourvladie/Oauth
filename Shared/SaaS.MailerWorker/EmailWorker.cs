using NLog;
using SaaS.Common.Extensions;
using SaaS.Data.Entities;
using SaaS.Data.Entities.Accounts;
using SaaS.Mailer;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace SaaS.MailerWorker
{
    public class EmailWorker : IEmailWorker
    {
        private static readonly int DefaultBatchSize = 10;

        private static readonly Logger _logger = LogManager.GetLogger("Logs");
        private static readonly Logger _errorsLogger = LogManager.GetLogger("Errors");

        public async Task StartAsync()
        {
            try
            {
                using (var repository = new Identity.EmailRepository())
                {
                    var emails = await repository.EmailsGetAsync(Status.NotStarted, DefaultBatchSize);

                    _logger.Info("emails in batch: {0}", emails.Count);

                    foreach (var email in emails)
                    {
                        _logger.Info("-----------------------------------------------------------------------");
                        await repository.EmailStatusSetAsync(email.Id, Status.InProcess);
                        await repository.EmailStatusSetAsync(email.Id, ProcessEmail(email));
                        _logger.Info("-----------------------------------------------------------------------");
                    }
                }

                GC.Collect();
            }
            catch (Exception exc)
            {
                _errorsLogger.Error(exc);
            }
        }

        private Status ProcessEmail(Email email)
        {
            try
            {
                var parser = new RazorParser(email.LanguageId);
                SendEmailAction entity = null;
                var entity0 = XMLSerializer.DeserializeObject<SendEmailAction2>(email.EmailCustomParam);
                
                //сделано чтоб залогировать miss templates matching
                if (entity0.EmailTemplateValue == EmailTemplate.None)
                {
                    throw new Exception("Couldn't find template: "  + entity0.EmailTemplateDBValue + ". Will use [" + entity0.TemplateId + "] template");
                }
                else
                {
                    entity = new SendEmailAction();
                    entity.EmailToList = entity0.EmailToList;
                    entity.TemplateId = entity0.EmailTemplateValue;
                }


                object model;
                string body = parser.ParseTemplateAsync(entity, email.ModelCustomParam, out model);
                string subject = parser.ParseSubject(entity, model);

                var templateName = Enum.GetName(typeof(EmailTemplate), entity.TemplateId);
                _logger.Info("try to send an email({0}): {1}", templateName, entity.EmailToList.FirstOrDefault().Email);
                EmailManager.Send(entity, subject, body);
                _logger.Info("email has been sent successfully: {0}", email.Id);

                return Status.Complete;
            }
            catch (Exception exc)
            {
                _logger.Info("email hasn't been sent successfully");
                _errorsLogger.Error(exc, "email hasn't been sent successfully: email.Id = {0}; Exc: {1}", email.Id, exc.Message);
                return Status.Error;
            }
        }
    }

    public interface IEmailWorker
    {
        Task StartAsync();
    }
}
