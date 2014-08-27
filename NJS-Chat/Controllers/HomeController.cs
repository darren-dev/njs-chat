using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NJS_Chat.Helpers;

namespace NJS_Chat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Message()
        {
            return View(MessageHelper.MessageQue);
        }

        [HttpPost]
        public ActionResult PostMessage(string message)
        {
            MessageHelper.Message bMessage = new MessageHelper.Message
            {
                DateSent = DateTime.Now,
                From = "Darren",
                To = "Everyone",
                MessageBody = message,
                MessageColor = MessageHelper.MessageColor.Green
            };


            MessageHelper mh = new MessageHelper();
            mh.QueMessage(bMessage);


            return RedirectToAction("Index");
        }
    }
}