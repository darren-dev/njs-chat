using System;
using System.Collections.Generic;
using System.Linq;

namespace NJS_Chat.Helpers
{
    public class UserHelper
    {
        #region Authorization
        public enum UserDetail
        {
            Valid,
            Inavlid,
            SessionExpired
        }

        public string AuthorizeUser(string username, string password)
        {
            string session;

            if (IsUserRegistered(username))
            {
                session = GetUserSession(username, password);
            }
            else
            {
                session = CreateUser(username, password);
            }

            return session;
        }

        public void UpdateUser(string username, Global.User user)
        {
            DatabaseHelper dh = new DatabaseHelper();
            dh.UpdateUser(username, user);
        }

        public UserDetail GetSessionState(string username, string sessionId)
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

        private static string GetUserSession(string username, string password)
        {
            DatabaseHelper dh = new DatabaseHelper();
            Global.User user = dh.GetUserFile(username);

            if (user == null)
            {
                return null;
            }

            if (user.Username == username && user.Password == password)
            {
                var session = CreateNewSessionForUser(user);

                return session;
            }

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
                Username = username
            };

            DatabaseHelper dh = new DatabaseHelper();
            dh.CreateUserFile(user);

            return session;
        }

        private static bool IsUserRegistered(string username)
        {
            DatabaseHelper dh = new DatabaseHelper();

            return dh.CheckIfUserFileExists(username);
        }
        #endregion

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

        internal void DequeUser(string username)
        {
            RemoveFromUserQue(username);
        }

        private static void RemoveFromUserQue(string username)
        {
            lock (Global.User.UserQue)
            {
                var que = GetUserQue();
                var toRemove = que.SingleOrDefault(x => x.Username == username);

                if (toRemove == null)
                {
                    return;
                }

                que.Remove(toRemove);
                Global.User.UserQue = que;
            }
        }

        private static void AddToUserQue(Global.User user)
        {
            lock (Global.User.UserQue)
            {
                var que = GetUserQue();

                que.Add(user);
                Global.User.UserQue = que;
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

        private static void AssertUserQueNotNull()
        {
            if (Global.User.UserQue == null)
            {
                Global.User.UserQue = new List<Global.User>();
            }
        }
    }
}