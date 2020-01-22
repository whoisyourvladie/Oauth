using System.Web.Mvc;

namespace SaaS.UI.Admin.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login() { return View(); }

        public ActionResult ChangePassword() { return View(); }
    }
}