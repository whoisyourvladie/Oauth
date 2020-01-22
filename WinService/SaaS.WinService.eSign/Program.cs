using System;

namespace SaaS.WinService.eSign
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (!Environment.UserInteractive)
            {
                var servicesToRun = new System.ServiceProcess.ServiceBase[]
                {
                    new eSignService()
                };

                System.ServiceProcess.ServiceBase.Run(servicesToRun);
            }
            else
            {
                using (var worker = new EmailChangeWorker())
                    worker.Do(10);
            }
        }
    }
}