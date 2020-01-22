using Quartz;
using Quartz.Impl;
using SaaS.MailerWorker.Jobs;

namespace SaaS.MailerWorker
{
    public class EmailScheduler : IEmailScheduler
    {
        private static readonly EmailScheduler _mailer = new EmailScheduler();
        private static IScheduler _scheduler;

        protected EmailScheduler()
        {

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler();

            IJobDetail regularJob =
                    JobBuilder.Create<RegularMailingJob>()
                        .WithIdentity("RegularMailing", "RegularMailingGroup")
                        .Build();

            ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("RegularMailingTrigger", "RegularMailingGroup")
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(15).RepeatForever())
                    .ForJob(regularJob)
                    .Build();

            AddJob(regularJob, trigger);
        }

        public static EmailScheduler GetMailer()
        {
            return _mailer;
        }

        public void AddJob(IJobDetail jobDetail, ITrigger trigger)
        {
            _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public bool RemoveJob(TriggerKey triggerKey)
        {
            return _scheduler.UnscheduleJob(triggerKey);
        }

        public void StartMailing()
        {
            _scheduler.Start();
        }

        public void StopMailing()
        {
            _scheduler.Shutdown();
        }

        public bool IsRunning
        {
            get { return _scheduler.IsStarted; }
        }

        public bool IsStopped
        {
            get { return _scheduler.IsShutdown; }
        }
    }

    public interface IEmailScheduler
    {
        bool IsRunning { get; }
        bool IsStopped { get; }
        void StartMailing();
        void StopMailing();
        void AddJob(IJobDetail jobDetail, ITrigger trigger);
        bool RemoveJob(TriggerKey triggerKey);
    }
}
