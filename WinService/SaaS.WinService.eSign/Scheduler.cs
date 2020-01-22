using SaaS.Identity;
using SaaS.WinService.Core;
using System;
using System.Timers;

namespace SaaS.WinService.eSign
{
    public class Scheduler : BaseScheduler
    {
        public override void Start()
        {
            new Timer() { Interval = TimeSpan.FromSeconds(10).TotalMilliseconds, Enabled = true }.Elapsed += delegate (object sender, ElapsedEventArgs e)
            {
                Do((Timer)sender, DoJob);
            };

            DoJob();
        }

        private void DoJob()
        {
            if (_queue.IsFull)
                return;

            _queue.AddTask(new EmptyEventHandler(EmailChange), new object[] { });
        }

        private void EmailChange()
        {
            _logger.Info("emailChange");

            try
            {
                var top = 10;

                using (var worker = new EmailChangeWorker())
                {
                    var count = worker.Do(top);

                    if (top == count)
                        EmailChange();
                }
            }
            catch (Exception exc)
            {
                _logger.Error(exc);
            }
        }
    }
}
