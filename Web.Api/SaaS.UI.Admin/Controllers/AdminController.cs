using System.Web.Mvc;

namespace SaaS.UI.Admin.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        [Authorize(Roles = "admin")]
        public ActionResult Logs() { return View(); }

        [Authorize(Roles = "admin")]
        public ActionResult Users() { return View(); }

        public ActionResult Roles() { return View(); }

        [Authorize(Roles = "admin")]
        public ActionResult Emails() { return View(); }
    }
}