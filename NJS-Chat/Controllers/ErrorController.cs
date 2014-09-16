using System.Web.Mvc;

namespace NJS_Chat.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult HttpError403(string error)
        {
            ViewBag.Description = error;
            return RedirectToAction("Login", "Auth");
        }

        public ActionResult HttpError404(string error)
        {
            ViewBag.Description = error;
            return RedirectToAction("Login", "Auth");
        }

        public ActionResult HttpError500(string error)
        {
            ViewBag.Description = error;
            return RedirectToAction("Login", "Auth");
        }

        public ActionResult GeneralError(string error)
        {
            ViewBag.Description = error;
            return RedirectToAction("Login", "Auth");
        }
    }
}