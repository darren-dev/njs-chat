using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NJS_Chat.Helpers;
using NJS_Chat.Models;

namespace NJS_Chat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string username)
        {
            IndexViewModel ivm = new IndexViewModel
            {
                Username = username
            };

            return View(ivm);
        }

        public ActionResult Message()
        {
            return View(MessageHelper.MessageQue);
        }

        [HttpPost]
        public ActionResult PostMessage(string username, string message)
        {
            MessageHelper.Message bMessage = new MessageHelper.Message
            {
                DateSent = DateTime.Now,
                From = username,
                To = "Everyone",
                MessageBody = message,
                MessageColor = MessageHelper.MessageColor.Green
            };


            MessageHelper mh = new MessageHelper();
            mh.QueMessage(bMessage);


            return RedirectToAction("Index", "Home", new { username = username });
        }
    }
}