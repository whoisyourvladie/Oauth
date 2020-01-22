using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaS.Api.Models.Oauth
{
    public class ExternalLoginSettingsViewModel
    {
        public ExternalLoginSettingsViewModel(ExternalLoginSettings google, ExternalLoginSettings facebook, ExternalLoginSettings microsoft)
        {
            Google = google;
            Facebook = facebook;
            Microsoft = microsoft;
        }

        public ExternalLoginSettings Google { get; set; }
        public ExternalLoginSettings Facebook { get; set; }
        public ExternalLoginSettings Microsoft { get; set; }
    }

    public class ExternalWindowSettings
    {
        public ExternalWindowSettings(Size size)
        {
            Width = size.Width;
            Height = size.Height;
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class ExternalLoginSettings
    {

        public ExternalWindowSettings Window { get; set; }
    }
}
