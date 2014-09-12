using System;
using System.Web;

namespace NJS_Chat.Helpers
{
    public class UserSessionHelper
    {
        internal static Global.UserSession GetUserSession()
        {
            Global.UserSession userSession = new Global.UserSession();

            if (HttpContext.Current.Session.Count == 0)
            {
                return userSession;
            }

            try
            {
                userSession.Username = HttpContext.Current.Session["_Username"].ToString();
            }
            catch
            {
                userSession.Username = null;
            }
            try
            {
                userSession.SessionId = HttpContext.Current.Session["_Session"].ToString();
            }
            catch
            {
                userSession.SessionId = null;
            }
            try
            {
                userSession.LoginDate = DateTime.Parse(HttpContext.Current.Session["_LoginTime"].ToString());
            }
            catch
            {
                userSession.LoginDate = default(DateTime);
            }
            try
            {
                userSession.LastMessageTime =
                    DateTime.Parse(HttpContext.Current.Session["_LastMessageTime"].ToString());
            }
            catch
            {
                userSession.LastMessageTime = default(DateTime);
            }

            return userSession;
        }

        internal void SetUserMessageTime()
        {
            HttpContext.Current.Session["_LastMessageTime"] = DateTime.UtcNow;
        }

        internal bool IsUserActive()
        {
            DateTime messageTime = DateTime.Parse(HttpContext.Current.Session["_LastMessageTime"].ToString()).AddMinutes(10);
            DateTime currentTime = DateTime.UtcNow;

            int result = DateTime.Compare(messageTime, currentTime);

            return result > 0;
        }
    }
}