using System;
using System.Web;
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
            string session = uh.AuthorizeUser(username, password);

            if (String.IsNullOrEmpty(session))
            {
                return RedirectToAction("Login");
            }
            HttpSessionStateWrapper sessionState = new HttpSessionStateWrapper(HttpContext.Current.Session);
            SessionService _SessionRepository = new SessionService(Session);
            uh.QueUser(username);

            return RedirectToAction("Index", "Home", new { username, sessionId = session });
        }
    }
}