using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NJS_Chat.Controllers;
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
            AssertNotNulls();
        }

        private void AssertNotNulls()
        {
            if (Global.User.UserQue == null)
            {
                Global.User.UserQue = new List<Global.User>();
            }

            if (Global.Message.MessageQue == null)
            {
                Global.Message.MessageQue = new List<Global.Message>();
            }
        }

        private static void InitiateGlobalQues()
        {
            Global.User.UserQue = new List<Global.User>();
            Global.User.KickQue = new List<string>();
            Global.Message.MessageQue = new List<Global.Message>();
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            HttpContext.Current.Session.Add("_Username", String.Empty);
            HttpContext.Current.Session.Add("_Session", String.Empty);
            HttpContext.Current.Session.Add("_LoginTime", DateTime.UtcNow);
            HttpContext.Current.Session.Add("_LastMessageTime", DateTime.UtcNow);
            HttpContext.Current.Session.Add("_UserGroup", DateTime.UtcNow);
        }

        protected void Session_End(object sender, EventArgs e)
        {
            string username = Session["_Username"].ToString();
            UserHelper uh = new UserHelper();
            uh.DequeUser(username, Application);
        }

        protected void Application_Error()
        {

            if (Context.IsCustomErrorEnabled)
            {
                ShowCustomErrorPage(Server.GetLastError());
            }

        }
        private void ShowCustomErrorPage(Exception exception)
        {
            var httpException = exception as HttpException ?? new HttpException(500, "Internal Server Error", exception);

            Response.Clear();
            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("fromAppErrorEvent", true);

            switch (httpException.GetHttpCode())
            {
                case 403:
                    routeData.Values.Add("action", "HttpError403");
                    break;

                case 404:
                    routeData.Values.Add("action", "HttpError404");
                    break;

                case 500:
                    routeData.Values.Add("action", "HttpError500");
                    break;

                default:
                    routeData.Values.Add("action", "GeneralError");
                    routeData.Values.Add("httpStatusCode", httpException.GetHttpCode());
                    break;
            }

            Server.ClearError();

            IController controller = new ErrorController();
            controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }
    }
}
