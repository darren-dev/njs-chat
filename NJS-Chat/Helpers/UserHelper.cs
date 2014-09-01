using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace NJS_Chat.Helpers
{
    public class UserHelper
    {

        public enum UserDetail
        {
            Valid,
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

        private string GetUserSession(string username, string password)
        {
            DatabaseHelper dh = new DatabaseHelper();
            Global.User user = dh.GetUserFile(username);

            if (user == null)
            {
                return null;
            }

            if (user.Username == username && user.Password == password)
            {
                return user.SessionId;
            }

            return null;
        }

        private string CreateUser(string username, string password)
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

        private bool IsUserRegistered(string username)
        {
            DatabaseHelper dh = new DatabaseHelper();

            return dh.CheckIfUserFileExists(username);
        }
    }
}