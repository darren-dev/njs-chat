using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            var session = UserSessionHelper.GetUserSession();
            if (String.IsNullOrEmpty(session.Username) || string.IsNullOrEmpty(session.SessionId))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (UserHelper.IsUserKicked(session.Username))
            {
                TempData["Errors"] = new List<String>
                {
                    "You have been kicked."
                };
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
                Selected = false,
                Text = user.Username,
                Value = user.Username
            }));

            IndexViewModel ivm = new IndexViewModel
            {
                ToSelectItems = items
            };

            return View(ivm);
        }
        
        public ActionResult Message()
        {
            UserSessionHelper ush = new UserSessionHelper();
            if (!ush.IsUserActive())
            {
                TempData["Errors"] = new List<String>
                {
                    "Logged out for inactivity"
                };

                return RedirectToAction("Login", "Auth");
            }

            var session = UserSessionHelper.GetUserSession();
            if (UserHelper.IsUserKicked(session.Username))
            {
                TempData["Errors"] = new List<String>
                {
                    "You have been kicked."
                };
                return Content("You have been kicked from the chat.");
            
            }


            MessageHelper mh = new MessageHelper();

            return View(mh.GetMessages());
        }

        [HttpPost]
        [Browsable(false)]
        public ActionResult PostMessage(IndexViewModel ivm)
        {
            if (ivm == null)
            {
                TempData["Errors"] = new List<String>
                {
                    "Session Expired"
                };
                return RedirectToAction("Login", "Auth");
            }

            UserSessionHelper ush = new UserSessionHelper();
            if (!ush.IsUserActive())
            {
                TempData["Errors"] = new List<String>
                {
                    "Logged out for inactivity"
                };
                return RedirectToAction("Login", "Auth");
            }

            var session = UserSessionHelper.GetUserSession();
            if (UserHelper.IsUserKicked(session.Username))
            {
                TempData["Errors"] = new List<String>
                {
                    "You have been kicked."
                };
                return RedirectToAction("Login", "Auth");
            }

            if (String.IsNullOrEmpty(session.Username) || string.IsNullOrEmpty(session.SessionId))
            {
                TempData["Errors"] = new List<String>
                {
                    "Session Expired"
                };
                return RedirectToAction("Login", "Auth");
            }

            if (String.IsNullOrWhiteSpace(ivm.Message))
            {
                return RedirectToAction("Index", "Home");
            }

            // Strip out HTML
            ivm.Message = Regex.Replace(ivm.Message, @"<[^>]*>", String.Empty);
            // Send message
            MessageHelper.SendMessage(ivm, session.Username);

            return RedirectToAction("Index", "Home");
        }
    }
}