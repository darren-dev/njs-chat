using System;
using System.Collections.Generic;
using System.Web;

namespace NJS_Chat.Helpers
{
    public static class Global
    {
        public class Message
        {
            public string MessageBody { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public DateTime DateSent { get; set; }
            public MessageColor MessageColor { get; set; }
            public static List<Message> MessageQue
            {
                get
                {
                    return HttpContext.Current.Application["MessageQue"] as List<Message>;
                }
                set
                {
                    HttpContext.Current.Application["MessageQue"] = value;
                }
            }
        }

        public enum MessageColor
        {
            White,
            Green,
            Red,
            Blue,
            Black,
            Orange,
            Pink
        }

        public class User
        {
            public string SessionId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public DateTime LoginDate { get; set; }
            public DateTime SessionTime { get; set; }

            public static List<User> UserQue
            {
                get
                {
                    return HttpContext.Current.Application["UserQue"] as List<User>;
                }
                set
                {
                    HttpContext.Current.Application["UserQue"] = value;
                }
            }
        }
    }
}