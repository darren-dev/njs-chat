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

            if (String.IsNullOrEmpty(sessionId))
            {
                return RedirectToAction("Login");
            }
            else
            {
                Session["_Username"] = username;
                Session["_Session"] = sessionId;
                uh.QueUser(username);
                return RedirectToAction("Index", "Home");
            }
        }
    }
}