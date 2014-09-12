using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NJS_Chat.Helpers;

namespace NJS_Chat
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            InitiateGlobalQues();
        }

        private static void InitiateGlobalQues()
        {
            Global.User.UserQue = new List<Global.User>();
            Global.Message.MessageQue = new List<Global.Message>();
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            HttpContext.Current.Session.Add("_Username", String.Empty);
            HttpContext.Current.Session.Add("_Session", String.Empty);
            HttpContext.Current.Session.Add("_LoginTime", DateTime.UtcNow);
            HttpContext.Current.Session.Add("_LastMessageTime", DateTime.UtcNow);
        }

        protected void Session_End(object sender, EventArgs e)
        {
            string username = Session["_Username"].ToString();
            UserHelper uh = new UserHelper();
            uh.DequeUser(username);
        }
    }
}
