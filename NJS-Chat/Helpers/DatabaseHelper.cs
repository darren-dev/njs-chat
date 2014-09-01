using System.IO;
using System.Web;
using System.Web.Script.Serialization;

namespace NJS_Chat.Helpers
{
    public class DatabaseHelper
    {
        public void CreateUserFile(Global.User user)
        {
            var serializer = new JavaScriptSerializer();
            var serializedList = serializer.Serialize(user);

            var path = HttpContext.Current.Server.MapPath("~/Database/" + user.Username + ".json");

            File.WriteAllText(@path, serializedList);
        }

        public bool CheckIfUserFileExists(string username)
        {
            var path = HttpContext.Current.Server.MapPath("~/Database/" + username + ".json");
            return File.Exists(path);
        }

        public Global.User GetUserFile(string username)
        {
            using (StreamReader r = new StreamReader("file.json"))
            {
                var serializer = new JavaScriptSerializer();

                string user = r.ReadToEnd();
                return serializer.Deserialize<Global.User>(user);
            }
        }

        public void UpdateUser(string username, Global.User user)
        {
            var serializer = new JavaScriptSerializer();
            var serializedList = serializer.Serialize(user);

            var path = HttpContext.Current.Server.MapPath("~/Database/" + user + ".json");

            File.WriteAllText(@path, serializedList);
        }
    }
}