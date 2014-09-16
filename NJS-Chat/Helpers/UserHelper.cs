using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NJS_Chat.Models;

namespace NJS_Chat.Helpers
{
    public class UserHelper
    {
        #region Authorization
        internal enum UserDetail
        {
            Valid,
            Inavlid,
            SessionExpired
        }

        internal string AuthorizeUser(string username, string password, out Global.UserGroup userGroup)
        {
            string session;

            if (IsUserRegistered(username))
            {
                userGroup = Global.UserGroup.Default;
                session = GetUserSession(username, password, out userGroup);
            }
            else
            {
                userGroup = Global.UserGroup.Default;
                session = CreateUser(username, password);
            }

            return session;
        }

        internal void UpdateUser(string username, Global.User user)
        {
            DatabaseHelper dh = new DatabaseHelper();
            dh.UpdateUser(username, user);
        }

        internal UserDetail GetSessionState(string username, string sessionId)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(sessionId))
            {
                return UserDetail.Inavlid;
            }

            DatabaseHelper dh = new DatabaseHelper();
            var user = dh.GetUserFile(username);

            if (sessionId != user.SessionId)
            {
                return UserDetail.Inavlid;
            }

            if (user.SessionTime <= DateTime.UtcNow.AddMinutes(-10))
            {
                return UserDetail.SessionExpired;
            }

            return UserDetail.Valid;
        }

        public void BanUser(string username, string banReason, DateTime? banLiftDate)
        {
            if (String.IsNullOrEmpty(username))
            {
                return;
            }


            DatabaseHelper dh = new DatabaseHelper();
            var user = dh.GetUserFile(username);
            if (user == null)
            {
                return;
            }

            user.IsBanned = true;
            user.BannedReason = banReason;
            user.BannedDate = DateTime.UtcNow;
            user.BannedLiftDate = banLiftDate;
            dh.UpdateUser(username, user);
        }

        internal void BlacklistUsername(string username)
        {
            DatabaseHelper dh = new DatabaseHelper();
            dh.AddUserToBlacklist(username);
        }

        internal bool IsUsernameBlacklisted(string username)
        {
            DatabaseHelper dh = new DatabaseHelper();
            return dh.GetAllBlacklistUsernames().Contains(username);
        }

        internal BanViewModel IsUserBanned(string username)
        {
            DatabaseHelper dh = new DatabaseHelper();
            var user = dh.GetUserFile(username);

            if (user != null)
            {
                BanViewModel bvm = new BanViewModel
                {
                    IsBanned = user.IsBanned,
                    UserName = username,
                    BanLiftDate = user.BannedLiftDate,
                    BanReason = user.BannedReason,
                    BannedDate = user.BannedDate
                };
                return bvm;
            }

            return new BanViewModel{IsBanned = false};
        }

        private static string GetUserSession(string username, string password, out Global.UserGroup userGroup)
        {
            DatabaseHelper dh = new DatabaseHelper();
            Global.User user = dh.GetUserFile(username);

            if (user == null)
            {
                userGroup = Global.UserGroup.Default;
                return null;
            }

            if (user.Username == username && user.Password == password)
            {
                var session = CreateNewSessionForUser(user);
                userGroup = user.UserGroup;

                return session;
            }

            userGroup = Global.UserGroup.Default;
            return null;
        }

        private static string CreateNewSessionForUser(Global.User user)
        {
            string session = Guid.NewGuid().ToString();
            user.SessionId = session;
            user.SessionTime = DateTime.UtcNow;
            UserHelper uh = new UserHelper();
            uh.UpdateUser(user.Username, user);
            return session;
        }

        private static string CreateUser(string username, string password)
        {
            string session = Guid.NewGuid().ToString();
            Global.User user = new Global.User
            {
                LoginDate = DateTime.UtcNow,
                Password = password,
                SessionId = session,
                SessionTime = DateTime.UtcNow,
                Username = username,
                IsBanned = false,
                UserGroup = Global.UserGroup.Default
            };

            DatabaseHelper dh = new DatabaseHelper();
            dh.CreateUserFile(user);

            return session;
        }

        private static bool IsUserRegistered(string username)
        {
            DatabaseHelper dh = new DatabaseHelper();

            return dh.CheckIfFileExists(username);
        }
        #endregion

        #region User
        internal void QueUser(Global.User user)
        {
            AssertUserQueNotNull();

            AddToUserQue(user);
        }

        internal void QueUser(string username)
        {
            AssertUserQueNotNull();

            DatabaseHelper dh = new DatabaseHelper();

            AddToUserQue(dh.GetUserFile(username));
        }

        internal void DequeUser(string username, HttpApplicationState application)
        {
            RemoveFromUserQue(username, application);
        }

        private static void RemoveFromUserQue(string username, HttpApplicationState application)
        {
            var que = GetUserQueByApplication(application);
            var toRemove = que.SingleOrDefault(x => x.Username == username);

            if (toRemove == null)
            {
                return;
            }

            que.Remove(toRemove);
            application["UserQue"] = que;
        }

        private static void AddToUserQue(Global.User user)
        {
            lock (Global.User.UserQue)
            {
                var que = GetUserQue();

                var curUser = que.FirstOrDefault(x => x.Username == user.Username && x.UserGroup != user.UserGroup);
                if (curUser != null)
                {
                    que.Remove(curUser);
                }

                if (!que.Exists(x => x.Username == user.Username))
                {
                    que.Add(user);
                    Global.User.UserQue = que.Distinct().ToList();
                }
            }
        }

        private static List<Global.User> GetUserQue()
        {
            AssertUserQueNotNull();

            lock (Global.User.UserQue)
            {
                List<Global.User> currUserQue =
                    Global.User.UserQue;
                return currUserQue;
            }
        }

        private static List<Global.User> GetUserQueByApplication(HttpApplicationState application)
        {
            return application["UserQue"] as List<Global.User>;
        }

        private static void AssertUserQueNotNull()
        {
            if (Global.User.UserQue == null)
            {
                Global.User.UserQue = new List<Global.User>();
            }
        }

        public static bool IsInGroup(params string[] groupName)
        {
            var userGroup = (Global.UserGroup) HttpContext.Current.Session["_UserGroup"];
            return groupName.Contains(userGroup.ToString());
        }

        public static bool IsInGroup(params Global.UserGroup[] groupName)
        {
            var userGroup = (Global.UserGroup)HttpContext.Current.Session["_UserGroup"];
            return groupName.Contains(userGroup);
        }
        #endregion

        #region Kick

        internal static void KickUser(string username)
        {
            AddToKickQue(username);
            MessageHelper mh = new MessageHelper();
            mh.QueMessage(new Global.Message
            {
                DateSent = DateTime.UtcNow,
                From = "SysBot",
                MessageBody = username + " kicked.",
                MessageColor = Global.MessageColor.Red,
                To = "Everyone"
            });
        }

        internal static bool IsUserKicked(string username)
        {
            bool isKicked = false;

            lock (Global.User.KickQue)
            {
                var que = GetKickQue();

                var curUser = que.FirstOrDefault(x => x.Equals(username));
                if (curUser != null)
                {
                    isKicked = true;
                    que.Remove(curUser);
                    HttpContext.Current.Session.Abandon();
                    Global.User.KickQue = que.Distinct().ToList();
                }
            }

            return isKicked;
        }

        private static void AddToKickQue(string username)
        {
            lock (Global.User.KickQue)
            {
                var que = GetKickQue();

                var curUser = que.FirstOrDefault(x => x.Equals(username));
                if (curUser != null)
                {
                    que.Remove(curUser);
                }

                if (!que.Exists(x => x.Equals(username)))
                {
                    que.Add(username);
                    Global.User.KickQue = que.Distinct().ToList();
                }
            }
        }
        private static List<String> GetKickQue()
        {
            AssertUserQueNotNull();

            lock (Global.User.KickQue)
            {
                List<String> currKickQue =
                    Global.User.KickQue;
                return currKickQue;
            }
        }

        private static void AssertUserKickNotNull()
        {
            if (Global.User.KickQue == null)
            {
                Global.User.KickQue = new List<String>();
            }
        }

        #endregion
    }
}