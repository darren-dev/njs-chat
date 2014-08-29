using System;
using System.Web.Mvc;
using NJS_Chat.Helpers;
using NJS_Chat.Models;

namespace NJS_Chat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string sessionId)
        {
            Global.User currentUser;
            UserHelper uh = new UserHelper();

            if (uh.AuthorizeUser(sessionId, out currentUser) == UserHelper.UserDetail.Valid)
            {
                IndexViewModel ivm = new IndexViewModel
                {
                    Username = currentUser.Username
                };

                return View(ivm);
            }

            return RedirectToAction("Login", "Auth");
        }

        public ActionResult Message()
        {
            MessageHelper mh = new MessageHelper();

            return View(mh.GetMessages());
        }

        [HttpPost]
        public ActionResult PostMessage(string sessionId, string message)
        {
            Global.User currentUser;
            UserHelper uh = new UserHelper();
            if (uh.AuthorizeUser(sessionId, out currentUser) != UserHelper.UserDetail.Valid)
            {
                return RedirectToAction("Login", "Auth");
            }

            Global.Message bMessage = new Global.Message
            {
                DateSent = DateTime.Now,
                From = currentUser.Username,
                To = "Everyone",
                MessageBody = message,
                MessageColor = Global.MessageColor.Green
            };


            MessageHelper mh = new MessageHelper();
            mh.QueMessage(bMessage);


            return RedirectToAction("Index", "Home", new { sessionId = sessionId });
        }
    }
}