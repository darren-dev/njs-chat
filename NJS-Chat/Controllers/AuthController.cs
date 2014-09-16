using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            // Validate username and password
            if (!ValidateUsername(username) || !ValidatePassword(password))
            {
                return RedirectToAction("Login");
            }

            UserHelper uh = new UserHelper();
            Global.UserGroup userGroup;
            var sessionId = uh.AuthorizeUser(username, password, out userGroup);

            if (String.IsNullOrEmpty(sessionId))
            {
                return RedirectToAction("Login");
            }
            else
            {
                Session["_Username"] = username;
                Session["_Session"] = sessionId;
                Session["_UserGroup"] = userGroup;
                uh.QueUser(username);
                return RedirectToAction("Index", "Home");
            }
        }

        private bool ValidatePassword(string password)
        {
            bool valid = true;
            var errors = TempData["Errors"] as List<String> ?? new List<string>();

            // Make sure the password is not empty
            if (String.IsNullOrEmpty(password))
            {
                errors.Add("Password can not be empty.");
                TempData["Errors"] = errors;

                return false;
            }

            TempData["Errors"] = errors;
            return valid;
        }

        private bool ValidateUsername(string username)
        {
            bool valid = true;
            var errors = TempData["Errors"] as List<String> ?? new List<string>();

            // Make sure the username is not empty
            if (String.IsNullOrEmpty(username))
            {
                errors.Add("Username can not be empty.");
                TempData["Errors"] = errors;

                return false;
            }

            // Make sure the username hasn't been blacklisted
            var uh = new UserHelper();
            if (uh.IsUsernameBlacklisted(username))
            {
                errors.Add("Username has been Blacklisted.");
                TempData["Errors"] = errors;

                return false;
            }

            var banStatus = uh.IsUserBanned(username);
            // Make sure the user isn't banned
            if (banStatus.IsBanned != null && (bool)banStatus.IsBanned)
            {
                if (banStatus.BannedDate.HasValue)
                {
                    errors.Add("User Banned on: " + banStatus.BannedDate.Value.ToLocalTime().ToString("F"));
                }
                else
                {
                    errors.Add("User Banned");
                }

                if (!String.IsNullOrEmpty(banStatus.BanReason))
                {
                    errors.Add("Ban Reason: " + banStatus.BanReason);
                }

                errors.Add("Banned until: " + (banStatus.BanLiftDate.HasValue ? banStatus.BanLiftDate.Value.ToLocalTime().ToString("D") : "Forever"));

                TempData["Errors"] = errors;

                return false;
            }

            // Make sure the name length is correct
            if (username.Length < 3 || username.Length > 20)
            {
                errors.Add("Username needs to be between 3 and 20 characters.");
                valid = false;
            }

            // Make sure name doesn't contain a number
            if (Regex.IsMatch(username, @"\d"))
            {
                errors.Add("Username can not contain a number.");
                valid = false;
            }



            TempData["Errors"] = errors;
            return valid;
        }
    }
}