using System;
using System.Web.Mvc;
using NJS_Chat.Helpers;

namespace NJS_Chat.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            UserHelper uh = new UserHelper();
            var sessionId = uh.AuthorizeUser(username, password);

            return String.IsNullOrEmpty(sessionId) ? RedirectToAction("Login") : RedirectToAction("Index", "Home", new {sessionId = sessionId});
        }
    }
}