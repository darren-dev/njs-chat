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

            return View(mh.GetMessages());
        }

        [HttpPost]
        public ActionResult PostMessage(string username, string sessionId, string message)
        {
            UserHelper uh = new UserHelper();
            if (uh.GetSessionState(username, sessionId) != UserHelper.UserDetail.Valid)
            {
                return RedirectToAction("Login", "Auth");
            }

            Global.Message bMessage = new Global.Message
            {
                DateSent = DateTime.Now,
                From = "Test",
                To = "Everyone",
                MessageBody = message,
                MessageColor = Global.MessageColor.Green
            };


            MessageHelper mh = new MessageHelper();
            mh.QueMessage(bMessage);


            return RedirectToAction("Index", "Home", new { username = username, sessionId = sessionId });
        }
    }
}