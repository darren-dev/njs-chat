using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;

namespace NJS_Chat.Helpers
{
    public class DatabaseHelper
    {
        #region General
        internal bool CheckIfFileExists(string username)
        {
            var path = HttpContext.Current.Server.MapPath("~/Database/" + username + ".json");
            return File.Exists(path);
        }

        internal void CreateFile(string fileName)
        {
            var path = HttpContext.Current.Server.MapPath("~/Database/" + fileName + ".json");
            File.WriteAllText(@path, String.Empty);
        }
        #endregion

        #region User
        internal void CreateUserFile(Global.User user)
        {
            var serializer = new JavaScriptSerializer();
            var serializedList = serializer.Serialize(user);

            var path = HttpContext.Current.Server.MapPath("~/Database/" + user.Username + ".json");

            File.WriteAllText(@path, serializedList);
        }

        internal Global.User GetUserFile(string username)
        {
            var path = HttpContext.Current.Server.MapPath("~/Database/" + username + ".json");
            if (!File.Exists(path))
            {
                return new Global.User();
            }
            using (StreamReader r = new StreamReader(path))
            {
                var serializer = new JavaScriptSerializer();

                var userJson = r.ReadToEnd();
                return serializer.Deserialize<Global.User>(userJson);
            }
        }

        internal Global.User GetUserFile(Global.User user)
        {
            var path = HttpContext.Current.Server.MapPath("~/Database/" + user.Username + ".json");
            if (!File.Exists(path))
            {
                return new Global.User();
            }
            using (StreamReader r = new StreamReader(path))
            {
                var serializer = new JavaScriptSerializer();

                string userJson = r.ReadToEnd();
                return serializer.Deserialize<Global.User>(userJson);
            }
        }

        internal void UpdateUser(string username, Global.User user)
        {
            var serializer = new JavaScriptSerializer();
            var serializedList = serializer.Serialize(user);

            var path = HttpContext.Current.Server.MapPath("~/Database/" + username + ".json");

            File.WriteAllText(@path, serializedList);

            // Rename file if the username changed
            if (username != user.Username)
            {
                var newPath = HttpContext.Current.Server.MapPath("~/Database/" + user.Username + ".json");
                File.Move(path, newPath);
            }
        }
        #endregion

        internal void AddUserToBlacklist(string username)
        {
            if (!CheckIfFileExists("_Blacklist"))
            {
                CreateFile("_Blacklist");
            }

            var path = HttpContext.Current.Server.MapPath("~/Database/_Blacklist.json");
            List<string> blacklist = GetAllBlacklistUsernames();

            if (!blacklist.Contains(username))
            {
                blacklist.Add(username);
            }


            var serializer = new JavaScriptSerializer();
            var serializedList = serializer.Serialize(blacklist);
            File.WriteAllText(@path, serializedList);
        }

        internal List<String> GetAllBlacklistUsernames()
        {
            if (!CheckIfFileExists("_Blacklist"))
            {
                CreateFile("_Blacklist");
            }

            var path = HttpContext.Current.Server.MapPath("~/Database/_Blacklist.json");
            var serializer = new JavaScriptSerializer();

            using (StreamReader r = new StreamReader(path))
            {
                string blacklistJson = r.ReadToEnd();
                var deserialized =  serializer.Deserialize<List<String>>(blacklistJson);

                return deserialized ?? new List<string>();
            }
        } 
    }
}