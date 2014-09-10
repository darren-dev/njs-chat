using System;
using System.Web.Mvc;
using NJS_Chat.Helpers;
using NJS_Chat.Models;

namespace NJS_Chat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string username, string sessionId)
        {
            UserHelper uh = new UserHelper();
            if (uh.GetSessionState(username, sessionId) != UserHelper.UserDetail.Valid)
            {
                return RedirectToAction("Login", "Auth");
            }

            IndexViewModel ivm = new IndexViewModel
            {
                Username = username
            };

            return View(ivm);
        }

        public ActionResult Message()
        {
            MessageHelper mh = new MessageHelper();
            MessageViewModel mvm = new MessageViewModel();
            mvm.Messages = mh.GetMessages();

            return View();
        }

        [HttpPost]
        public ActionResult PostMessage(string username, string message, string to)
        {
            UserHelper uh = new UserHelper();

            if (Request.UrlReferrer == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Request.UrlReferrer.Query.IndexOf("sessionId", StringComparison.Ordinal) == -1)
            {
                return RedirectToAction("Login", "Auth");
            }

            string sessionId = Request.UrlReferrer.Query.Substring(Request.UrlReferrer.Query.IndexOf("sessionId", StringComparison.Ordinal) + 10, 36);
            if (uh.GetSessionState(username, sessionId) != UserHelper.UserDetail.Valid)
            {
                return RedirectToAction("Login", "Auth");
            }

            Global.Message bMessage = new Global.Message
            {
                DateSent = DateTime.UtcNow,
                From = username,
                To = to,
                MessageBody = message,
                MessageColor = Global.MessageColor.Green
            };


            MessageHelper mh = new MessageHelper();
            mh.QueMessage(bMessage);


            return RedirectToAction("Index", "Home", new { username, sessionId });
        }

    }
}