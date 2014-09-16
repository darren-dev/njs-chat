using System.Web.Mvc;
using NJS_Chat.Helpers;
using NJS_Chat.Models;

namespace NJS_Chat.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult KickUser(string username)
        {
            UserHelper.KickUser(username);

            return RedirectToAction("Message", "Home");
        }

        public ActionResult BlacklistUserName(string username)
        {
            UserHelper uh = new UserHelper();
            uh.BlacklistUsername(username);
            UserHelper.KickUser(username);

            return RedirectToAction("Message", "Home");
        }

        public ActionResult BanUser(string username)
        {
            TempData["cur_username"] = username;
            BanViewModel bvm = new BanViewModel
            {
                UserName = username
            };

            return View(bvm);
        }

        [HttpPost]
        public ActionResult BanUser(BanViewModel bvm)
        {
            UserHelper uh = new UserHelper();
            uh.BanUser(TempData["cur_username"].ToString(), bvm.BanReason, bvm.BanLiftDate);
            UserHelper.KickUser(TempData["cur_username"].ToString());

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