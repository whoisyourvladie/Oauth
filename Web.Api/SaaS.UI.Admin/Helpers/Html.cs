using Newtonsoft.Json;
using SaaS.UI.Admin.App_Start;
using StructureMap;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI;

namespace SaaS.UI.Admin.Helpers
{
    public static class Html
    {
        private static readonly IHtmlString _active = new HtmlString("active");

        public static bool IsActive(this HtmlHelper html, string controller)
        {
            string _controller = (string)html.ViewContext.RouteData.Values["controller"];

            return string.Compare(_controller, controller, true) == 0;
        }
        public static IHtmlString IsActiveClass(this HtmlHelper html, string controller, string action)
        {
            string _controller = (string)html.ViewContext.RouteData.Values["controller"];
            string _action = (string)html.ViewContext.RouteData.Values["action"];

            if (string.Compare(_controller, controller, true) == 0 && string.Compare(_action, action, true) == 0)
                return new HtmlString("active");

            return null;
        }
        public static IHtmlString IsActiveClass(this HtmlHelper html, string controller)
        {
            return html.IsActive(controller) ? _active : null;
        }
        public static IHtmlString IsNotActiveClass(this HtmlHelper html, string controller)
        {
            string _controller = (string)html.ViewContext.RouteData.Values["controller"];

            if (string.Compare(_controller, controller, true) != 0)
                return new HtmlString("active");

            return null;
        }

        public static IHtmlString PageHeader(this HtmlHelper html, string title)
        {
            using (HtmlTextWriter writer = new HtmlTextWriter(html.ViewContext.Writer, string.Empty))
            {
                //< div class="row">
                //    <div class="col-lg-12">
                //        <h1 class="page-header">Roles</h1>
                //    </div>
                //</div>

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "row");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "col-lg-12");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "page-header");
                writer.RenderBeginTag(HtmlTextWriterTag.H1);
                writer.Write(title);

                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderEndTag();
            }

            return null;

        }
        public static IHtmlString AppSettings(this HtmlHelper html)
        {
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            var user = HttpContext.Current.User;

            string controller = (string)html.ViewContext.RouteData.Values["controller"];
            string action = (string)html.ViewContext.RouteData.Values["action"];

            IAppSettings settings = ObjectFactory.GetInstance<IAppSettings>();

            var appSettings = new
            {
                lang = "en",
                debug = HttpContext.Current.IsDebuggingEnabled,
                oauth = settings.OAuth,
                page = new
                {
                    controller = controller,
                    action = action
                },
                user = new
                {
                    identity = new
                    {
                        name = user.Identity.Name,
                        isAuthenticated = user.Identity.IsAuthenticated
                    }
                }
            };

            string json = Serialize(appSettings);

            using (HtmlTextWriter writer = new HtmlTextWriter(html.ViewContext.Writer))
            {
                string script = string.Format("angular.module('app').value('appSettings', {0});", json);

                writer.WriteScript(script);
            }

            return null;
        }
        private static void WriteScript(this HtmlTextWriter writer, string script)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.Write(script);
            writer.RenderEndTag();
        }
        private static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings);
        }
        public static IHtmlString Special(this HtmlHelper html)
        {
            var user = HttpContext.Current.User;

#if LuluSoft
            #region

            var special = new
            {
                url = "https://cgate.sodapdf.com/join.aspx",
                tracking = new
                {
                    wid = 6644,
                    uid = 1015225,
                    @ref = "sodapdf.com/support",
                    cmp = "spdf_all_support_all_all_all_all",
                    key1 = "support",
                    ga = "UA-17191366-1",
                    gtm = "GTM-52QP9R",
                    mkey1 = user.Identity.Name,
                    mkey2 = user.Identity.Name
                },
                products = new SpecialProduct[]
               {
                    new SpecialProduct("Soda PDF Anywhere Home(Yearly)")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "77iPNOyuq2k="), new SpecialDiscount("50%", "ugRb4nrSAzA=") }
                    },
                    new SpecialProduct("Soda PDF Anywhere Premium (Yearly)")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "ELY1bRxSToE="), new SpecialDiscount("50%", "eS0Fup0MdgE=") }
                    },
                    new SpecialProduct("Soda PDF Anywhere Business (Yearly)")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "+uzT/KF1LWE="), new SpecialDiscount("50%", "tbdX/7UxkUw=") }
                    },
                    new SpecialProduct("Soda PDF 10 Home")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "Agw4iVbg790="), new SpecialDiscount("50%", "Tg0/FBi+QHs=") }
                    },
                    new SpecialProduct("Soda PDF 10 Home + OCR")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "2SzOiUkbXzA="), new SpecialDiscount("50%", "k8mvmgOyJXc=") }
                    },
                    new SpecialProduct("Soda PDF 10 Premium")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "uATpR9Wb0w4="), new SpecialDiscount("50%", "UE/t8J7I9q0=") }
                    },
                    new SpecialProduct("Soda PDF 10 Premium + OCR")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "HSdgi1K/qgc="), new SpecialDiscount("50%", "VQfEVXF0MJ4=") }
                    },



                    new SpecialProduct("Soda PDF 11 Home")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "vtUv1ULcsQ0="), new SpecialDiscount("50%", "jjVUfpm56GU=") }
                    },
                    new SpecialProduct("Soda PDF 11 Home + OCR")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "%2BE7LOPcO7rQ="), new SpecialDiscount("50%", "Jx29FsB5h3c=") }
                    },
                    new SpecialProduct("Soda PDF 11 Premium")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "VQOL4TZbJ08="), new SpecialDiscount("50%", "gXU9HkLlc08=") }
                    },
                    new SpecialProduct("Soda PDF 11 Premium + OCR")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "UKWmwj5ylIQ="), new SpecialDiscount("50%", "kAxE3bow7u4=") }
                    },
                    new SpecialProduct("Soda PDF 11 Business")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "1YMvd7Jp6dI="), new SpecialDiscount("50%", "aIAxsjAiqqI=") }
                    },


                    new SpecialProduct("E-Sign Unlimited")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("0%", "VvnIFGaHq50=") }
                    },
                    new SpecialProduct("OCR Add-On")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("0%", "p/wXwkfK3lw=") }
                    },
                    new SpecialProduct("E-Sign 10-Pack")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("0%", "6qxF7t5DG24=") }
                    },
                    new SpecialProduct("Soda PDF Back Up Protection")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("0%", "5OuYuAZ8ypw=") }
                    },
                    new SpecialProduct("Extended Download Protection")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("0%", "D2cAHsJxzRs=") }
                    },
                    new SpecialProduct("Back Up CD")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("0%", "qL5WIe1VucE=") }
                    }
               }
            };

            #endregion
#endif

#if PdfForge
            #region

            var special = new
            {
                url = "https://cgate.pdfarchitect.org/join.aspx",
                tracking = new
                {
                    wid = 3843,
                    uid = 1006694,
                    @ref = "pdfarchitect.org",
                    cmp = "pdfa_all_support_all_all_all_all",
                    key1 = "support",
                    ga = "UA-36447566-1",
                    gtm = "GTM-W39F3W",
                    mkey1 = user.Identity.Name,
                    mkey2 = user.Identity.Name
                },
                products = new SpecialProduct[]
                {
                    new SpecialProduct("PDF Architect 6 Standard Edition")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "ojTLzn6zFqM="), new SpecialDiscount("50%", "jz0iCLAp5rk=") }
                    },
                    new SpecialProduct("PDF Architect 6 Pro Edition")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "A3INEjQpuvs="), new SpecialDiscount("50%", "bYN2KRCp8Nk=") }
                    },
                    new SpecialProduct("PDF Architect 6 Pro+OCR Edition")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "lO0vJXbW5ho="), new SpecialDiscount("50%", "cbHf4zBAyzA=") }
                    },
                    new SpecialProduct("PDF Architect Standard Plan")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "jNpQGlTpprQ="), new SpecialDiscount("50%", "H3QXpieXBZM=") }
                    },
                    new SpecialProduct("PDF Architect Pro Plan")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "XIQ1xx+TX7M="), new SpecialDiscount("50%", "oqjlY9T+W3I=") }
                    },
                    new SpecialProduct("PDF Architect Pro+OCR Plan")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "Om1SD+OdEuk="), new SpecialDiscount("50%", "/xWSGI3sfd4=") }
                    },
                    new SpecialProduct("PDF Architect - OCR Plan")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "+6YgbNSTjr0="), new SpecialDiscount("50%", "6K+QPV/V3rQ=") }
                    },
                    new SpecialProduct("PDF Architect Convert Plan")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "+tY3JM8QVx8=") }
                    },
                    new SpecialProduct("PDF Architect Edit Plan")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "Ncejr5pa8bw=") }
                    },
                    new SpecialProduct("PDF Architect E-Sign Unlimited")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("25%", "yh652FTyy6o="), new SpecialDiscount("50%", "mOfywTIeFIA=") }
                    },
                    new SpecialProduct("Extended Download Protection (for plans)")
                    {
                        Discounts = new SpecialDiscount[] { new SpecialDiscount("0%", "ziKjRhoLgJQ=") }
                    }
                }
            };

            #endregion
#endif

#if PdfSam

            var special = new
            {
                url = "https://cgate.sodapdf.com/join.aspx",
                tracking = new
                {
                    mkey1 = user.Identity.Name,
                    mkey2 = user.Identity.Name
                },
                products = new SpecialProduct[]
                {

                }
            };

#endif

            string json = Serialize(special);
            using (HtmlTextWriter writer = new HtmlTextWriter(html.ViewContext.Writer))
            {
                string script = string.Format("angular.module('app').value('special', {0});", json);
                writer.WriteScript(script);
            }

            return null;
        }

        private class SpecialProduct
        {
            public SpecialProduct(string name)
            {
                Name = name;
            }

            public string Name { get; set; }
            public SpecialDiscount[] Discounts { get; set; }
        }

        private class SpecialDiscount
        {
            public SpecialDiscount(string name, string ujId)
            {
                Name = name;
                UjId = ujId;
            }

            public string Name { get; set; }
            public string UjId { get; set; }
        }
    }
}