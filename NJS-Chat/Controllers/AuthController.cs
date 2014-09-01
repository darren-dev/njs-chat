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
            uh.AuthorizeUser(username, password);

            return null;
        }
    }
}