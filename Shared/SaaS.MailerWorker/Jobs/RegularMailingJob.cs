using NLog;
using Quartz;
using System;

namespace SaaS.MailerWorker.Jobs
{
    [DisallowConcurrentExecution]
    public class RegularMailingJob : IMailingJob
    {
        private static readonly Logger _logger = LogManager.GetLogger("Logs");
        private static readonly Logger _errorsLogger = LogManager.GetLogger("Errors");

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                new EmailWorker().StartAsync().Wait();
            }
            catch (Exception exc)
            {
                _errorsLogger.Error(exc);
                throw new JobExecutionException(exc);
            }
        }
    }

    public interface IMailingJob : IJob { }
}
