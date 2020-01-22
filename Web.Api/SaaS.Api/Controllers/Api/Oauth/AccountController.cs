using Newtonsoft.Json.Linq;
using SaaS.Api.Models.Products;
using SaaS.Data.Entities.View;
using SaaS.ModuleFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using ValidateAntiForgeryToken = System.Web.Mvc.ValidateAntiForgeryTokenAttribute;

namespace SaaS.Api.Controllers.Api.Oauth
{
    [RoutePrefix("api/account"), ValidateAntiForgeryToken]
    public partial class AccountController : SaaSApiController
    {
        private static Map _moduleFeaturesMap;
        private static string _defaultPreferencesJson;
        private static readonly HashSet<string> _b2bleadSourceCollection;

        static AccountController()
        {
            _b2bleadSourceCollection = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

#if LuluSoft
            _b2bleadSourceCollection.Add("sodapdf.com-request-quote");
            _b2bleadSourceCollection.Add("sodapdf.com-get-trial");
            _b2bleadSourceCollection.Add("sodapdf.com-contact-us");
            _b2bleadSourceCollection.Add("sodapdf.com-webinar");
            _b2bleadSourceCollection.Add("sodapdf.com-white-paper");
            _b2bleadSourceCollection.Add("sodapdf.com-reseller");
#endif

            var defaultPreferencesFileInfo = new FileInfo(DefaultPreferencesFilePath);
            _defaultPreferencesJson = defaultPreferencesFileInfo.Exists ? File.ReadAllText(DefaultPreferencesFilePath) : "{}";

            _moduleFeaturesMap = string.IsNullOrEmpty(ModuleFeaturesFilePath) ? null : Map.Load(ModuleFeaturesFilePath);
        }

        private static JObject DefaultPreferences()
        {
            return JObject.Parse(_defaultPreferencesJson);
        }
        private static string[] GetJObjectProperties(JObject jObject)
        {
            return jObject.Properties()
                .Select(property => property.Name)
                .ToArray();
        }
        private static string DefaultPreferencesFilePath
        {
            get
            {
#if LuluSoft
                return HostingEnvironment.MapPath("~/bin/App_GlobalResources/luluSoft-defaultPreferences.json");
#endif

#if PdfForge
                return HostingEnvironment.MapPath("~/bin/App_GlobalResources/pdfForge-defaultPreferences.json");
#endif

#if PdfSam
                return HostingEnvironment.MapPath("~/bin/App_GlobalResources/pdfSam-defaultPreferences.json");
#endif
            }
        }
        private static string ModuleFeaturesFilePath
        {
            get
            {
#if LuluSoft
                return HostingEnvironment.MapPath("~/bin/App_GlobalResources/luluSoft-moduleFeatures.json");
#endif

#if PdfForge
                return HostingEnvironment.MapPath("~/bin/App_GlobalResources/pdfForge-moduleFeatures.json");
#endif

#if PdfSam
                return HostingEnvironment.MapPath("~/bin/App_GlobalResources/pdfSam-moduleFeatures.json");
#endif
            }
        }

        internal static MapModule[] GetModuleFeatures(ViewAccountProduct product, List<AccountProductModuleModel> modules)
        {
            if (object.Equals(_moduleFeaturesMap, null))
                return null;

            Version productVersion;
            Version.TryParse(product.ProductVersion, out productVersion);

            var mapStrategy = product.IsPPC ? MapStrategy.PerpetualLicense : MapStrategy.Subscription;
            var moduleFeatures = _moduleFeaturesMap.GetModules(mapStrategy, productVersion);

            if (!object.Equals(modules, null) && !object.Equals(moduleFeatures, null))
            {
                foreach (var module in modules)
                {
                    var moduleFeature = moduleFeatures.FirstOrDefault(e => string.Equals(e.Name, module.Module, StringComparison.InvariantCultureIgnoreCase));
                    if (!object.Equals(moduleFeature, null))
                        moduleFeature.ActivateFeatures();
                }
            }

            return moduleFeatures;
        }
    }
}