using NLog;
using System.ServiceProcess;

namespace SaaS.WinService.eSign
{
    public partial class eSignService : ServiceBase
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public eSignService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _logger.Info("OnStart");

            Scheduler scheduler = new Scheduler();
            scheduler.Start();

            _logger.Info("OnStart Complete");
        }

        protected override void OnStop()
        {
            _logger.Info("OnStop");
        }
    }
}
