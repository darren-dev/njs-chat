using System;

namespace NJS_Chat.Helpers
{
    public class UserHelper
    {
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

            if (user.SessionTime <= DateTime.Now.AddMinutes(-10))
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
            user.SessionTime = DateTime.Now;
            UserHelper uh = new UserHelper();
            uh.UpdateUser(user.Username, user);
            return session;
        }

        private static string CreateUser(string username, string password)
        {
            string session = Guid.NewGuid().ToString();
            Global.User user = new Global.User
            {
                LoginDate = DateTime.Now,
                Password = password,
                SessionId = session,
                SessionTime = DateTime.Now,
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
    }
}