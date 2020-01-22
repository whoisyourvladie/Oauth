using SaaS.MailerWorker;
using System.ServiceProcess;

namespace SaaS.WinService.Mailer
{
    public partial class MailerService : ServiceBase
    {
        private readonly EmailScheduler _mailer;
        public MailerService()
        {
            InitializeComponent();
            _mailer = EmailScheduler.GetMailer();
        }

        protected internal void StartUserInteractive()
        {
            OnStart(new string[] { });
        }

        protected override void OnStart(string[] args)
        {
            _mailer.StartMailing();
        }

        protected override void OnStop()
        {
            _mailer.StopMailing();
        }
    }
}
