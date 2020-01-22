using System;
using System.ServiceProcess;
using System.Threading;

namespace SaaS.WinService.Mailer
{
    static class Program
    {
        static void Main()
        {
            if (!Environment.UserInteractive)
            {
                var servicesToRun = new ServiceBase[]
                {
                    new MailerService()
                };
                ServiceBase.Run(servicesToRun);
            }
            else
            {
                MailerService mailerService = new MailerService();
                mailerService.StartUserInteractive();
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
