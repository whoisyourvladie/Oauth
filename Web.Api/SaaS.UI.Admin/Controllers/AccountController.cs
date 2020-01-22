using System.Web.Mvc;

namespace SaaS.UI.Admin.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public ActionResult Details() { return View(); }
        public ActionResult Index() { return View(); }

        public ActionResult Search() { return View(); }

        public ActionResult Password() { return View(); }

        public ActionResult Products() { return View(); }

        [ActionName("Add-Product"), Authorize(Roles = "manager,admin")]
        public ActionResult AddProduct()
        {
            return View("AddProduct");
        }

        public ActionResult Special() { return View(); }

        public ActionResult Register() { return View(); }
    }
}