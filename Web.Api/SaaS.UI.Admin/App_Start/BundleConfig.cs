using System.Web.Optimization;

namespace SaaS.UI.Admin
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundle/css")

#if LuluSoft
              .Include("~/css/bootstrap/default/bootstrap.min.css") //https://bootswatch.com/3/
              .Include("~/css/theme/luluSoft/style.css")
#endif

#if PdfForge
              .Include("~/css/bootstrap/lumen/bootstrap.min.css") //https://bootswatch.com/3/
              .Include("~/css/theme/pdfForge/style.css")
#endif

#if PdfSam
              .Include("~/css/bootstrap/paper/bootstrap.min.css") //https://bootswatch.com/3/
              .Include("~/css/theme/pdfSam/style.css")
#endif

              .Include("~/css/font-awesome/css/fontawesome-all.min.css")
              .Include("~/css/metisMenu/metisMenu.min.css")
              .Include("~/css/notify/css/angular-notify-bordered.min.css")
              .Include("~/css/style.css")
              .Include("~/css/validation.css")
            );

            string pattern = "*.js";

            bundles.Add(new ScriptBundle("~/bundle/js")
                .Include("~/js/bundle.js")
                .Include("~/js/app.js")
                .IncludeDirectory("~/js/auth", pattern, true)
                .IncludeDirectory("~/js/services", pattern, true)
                .IncludeDirectory("~/js/directives", pattern, true)
                //.IncludeDirectory("~/js/filters", pattern, true)
                .IncludeDirectory("~/js/controllers", pattern, true)
            );
        }
    }
}
