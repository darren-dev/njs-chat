using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NJS_Chat.Helpers
{
    public class UserHelper
    {
        public string AuthorizeUser(string username, string password)
        {
            return IsUserInQue(username) ? LoginUser(username, password) : RegisterUser(username, password);
        }

        public UserDetail AuthorizeUser(string sessionId, out Global.User user)
        {
            user = new Global.User();
            if (HasUserSessionExpired(sessionId))
            {
                user = GetUserBySessionId(sessionId);
                return UserDetail.SessionExpired;
            }

            return UserDetail.Valid;
        }

        private bool HasUserSessionExpired(string sessionId)
        {
            var user = GetUserBySessionId(sessionId);
            var curDate = DateTime.Now;

            if (user == null) return true;

            if (user.SessionTime <= curDate.AddMinutes(-10))
            {
                return true;
            }

            var newUser = user;
            newUser.SessionTime = curDate;

            UpdateSingleUser(user, newUser);

            return false;
        }

        private void UpdateSingleUser(Global.User user, Global.User newUser)
        {
            if (String.IsNullOrEmpty(DatabaseFile)) return;

            lock (DatabaseFile)
            {
                List<Global.User> currUserQue = HttpContext.Current.Application["UserQue"] as List<Global.User>;
                if (currUserQue == null) return;

                currUserQue.Remove(user);
                currUserQue.Add(newUser);

                HttpContext.Current.Application["UserQue"] = currUserQue;
            }
        }

        private string RegisterUser(string username, string password)
        {
            AssertUserQueNotNull();

            Global.User user = CreateUser(username, password);

            var currUserQue = GetUserQue();
            if (currUserQue == null) return null;

            AddToUserQue(user, currUserQue);

            return user.SessionId;
        }
        private string LoginUser(string username, string password)
        {
            AssertUserQueNotNull();

            var currUserQue = GetUserQue();

            var user = currUserQue.FirstOrDefault(x => x.Password == password);
            if (user == null) return null;
            if (user.Username != username) return null;

            var currSession = Guid.NewGuid().ToString();

            var curDate = DateTime.Now;
            user.SessionId = currSession;
            user.LoginDate = curDate;
            user.SessionTime = curDate;

            UpdateUserQue(currUserQue);

            return currSession;
        }

        private static Global.User CreateUser(string username, string password)
        {
            var curDate = DateTime.Now;
            return new Global.User
            {
                Username = username, 
                Password = password,
                LoginDate = curDate,
                SessionId = Guid.NewGuid().ToString(),
                SessionTime = curDate
            };
        }

        private static void AddToUserQue(Global.User user, List<Global.User> currUserQue)
        {
            lock (HttpContext.Current.Application["UserQue"])
            {
                lock (currUserQue)
                {
                    currUserQue.Add(user);
                    HttpContext.Current.Application["UserQue"] = currUserQue;
                }
            }
        }

        private static List<Global.User> GetUserQue()
        {
            lock (HttpContext.Current.Application["UserQue"])
            {
                List<Global.User> currUserQue = HttpContext.Current.Application["UserQue"] as List<Global.User>;
                return currUserQue;
            }
        }

        private static void UpdateUserQue(List<Global.User> userQue)
        {
            lock (HttpContext.Current.Application["UserQue"])
            {
                HttpContext.Current.Application["UserQue"] = userQue;
            } 
        }

        private static bool IsUserInQue(string username)
        {
            if (HttpContext.Current.Application["UserQue"] == null) return false;

            lock (HttpContext.Current.Application["UserQue"])
            {
                List<Global.User> currUserQue = HttpContext.Current.Application["UserQue"] as List<Global.User>;
                if (currUserQue == null) return false;

                var user = currUserQue.FirstOrDefault(x => x.Username == username);

                return user != null;
            }
        }

        private Global.User GetUserBySessionId(string sessionId)
        {
            if (HttpContext.Current.Application["UserQue"] == null) return null;

            lock (HttpContext.Current.Application["UserQue"])
            {
                List<Global.User> currUserQue = HttpContext.Current.Application["UserQue"] as List<Global.User>;
                if (currUserQue == null) return null;

                var user = currUserQue.FirstOrDefault(x => x.SessionId == sessionId);

                return user;
            }
        }

        private static void AssertUserQueNotNull()
        {
            if (HttpContext.Current.Application["UserQue"] == null)
            {
                HttpContext.Current.Application["UserQue"] = new List<Global.User>();
            }
        }

        public enum UserDetail
        {
            Valid,
            SessionExpired
        }
    }
}