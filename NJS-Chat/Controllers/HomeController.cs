using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using NJS_Chat.Helpers;
using NJS_Chat.Models;

namespace NJS_Chat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string username = Session["_Username"].ToString();
            string sessionId = Session["_Session"].ToString();

            UserHelper uh = new UserHelper();
            if (uh.GetSessionState(username, sessionId) != UserHelper.UserDetail.Valid)
            {
                return RedirectToAction("Login", "Auth");
            }

            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Selected = true,
                    Text = "Everyone",
                    Value = "Everyone"
                }
            };
            items.AddRange(Global.User.UserQue.Select(user => new SelectListItem
            {
                Selected = false, Text = user.Username, Value = user.Username
            }));

            IndexViewModel ivm = new IndexViewModel
            {
                Username = username,
                Session = sessionId,
                ToSelectItems = items
            };

            return View(ivm);
        }

        public ActionResult Message()
        {
            MessageHelper mh = new MessageHelper();
            MessageViewModel mvm = new MessageViewModel();
            mvm.Messages = mh.GetMessages();

            return View(mh.GetMessages());
        }

        [HttpPost]
        public ActionResult PostMessage(IndexViewModel ivm)
        {
            if (ivm == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            UserHelper uh = new UserHelper();

            if (uh.GetSessionState(ivm.Username, ivm.Session) != UserHelper.UserDetail.Valid)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (String.IsNullOrWhiteSpace(ivm.Message))
            {
                return RedirectToAction("Index", "Home");
            }

            // Strip out HTML
            ivm.Message = Regex.Replace(ivm.Message, @"<[^>]*>", String.Empty);
            // Send message
            SendMessage(ivm);

            return RedirectToAction("Index", "Home");
        }

        private static void SendMessage(IndexViewModel ivm)
        {
            Global.Message bMessage = new Global.Message
            {
                DateSent = DateTime.UtcNow,
                From = ivm.Username,
                To = ivm.To,
                MessageBody = ivm.Message.Trim(),
                MessageColor = Global.MessageColor.Green
            };


            MessageHelper mh = new MessageHelper();
            mh.QueMessage(bMessage);
        }
    }
}