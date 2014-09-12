using System.Web.Mvc;
using NJS_Chat.Helpers;

namespace NJS_Chat.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult BlacklistUser(string username)
        {
            UserHelper uh = new UserHelper();
            uh.BlacklistUsername(username);
            return RedirectToAction("Message", "Home");
        }

        public ActionResult BanUser(string username)
        {
            UserHelper uh = new UserHelper();
            uh.BanUser(username);
            return RedirectToAction("Message", "Home");
        }

        public ActionResult PromoteUser(string username)
        {

            return RedirectToAction("Message", "Home");
        }

        public ActionResult DemoteUser(string username)
        {

            return RedirectToAction("Message", "Home");
        }
    }
}