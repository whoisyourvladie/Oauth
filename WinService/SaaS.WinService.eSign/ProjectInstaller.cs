using System.Collections;
using System.ComponentModel;

namespace SaaS.WinService.eSign
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            SetServiceName();
            base.OnBeforeInstall(savedState);
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            SetServiceName();
            base.OnBeforeUninstall(savedState);
        }

        private void SetServiceName()
        {
            if (Context.Parameters.ContainsKey("ServiceName"))
                this.serviceInstaller.ServiceName = Context.Parameters["ServiceName"];

            if (Context.Parameters.ContainsKey("DisplayName"))
                this.serviceInstaller.DisplayName = Context.Parameters["DisplayName"];
        }
    }
}
