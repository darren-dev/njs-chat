using System;
using System.Collections.Generic;
using System.Web;

namespace NJS_Chat.Helpers
{
    public static class Global
    {
        public const int MaxIdleTime = 15;


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

        public enum UserGroup
        {
            Default,
            Moderator,
            Administrator
        }

        public class User
        {
            public string SessionId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public DateTime LoginDate { get; set; }
            public DateTime SessionTime { get; set; }
            public Boolean IsBanned { get; set; }
            public DateTime BannedDate { get; set; }
            public string BannedReason { get; set; }
            public DateTime? BannedLiftDate { get; set; }
            public UserGroup UserGroup { get; set; }

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

            public static List<String> KickQue
            {
                get
                {
                    return HttpContext.Current.Application["KickQue"] as List<String>;
                }
                set
                {
                    HttpContext.Current.Application["KickQue"] = value;
                }
            } 
        }

        internal class UserSession : User
        {
            internal DateTime LoginTime { get; set; }
            internal DateTime LastMessageTime { get; set; }
        }
    }
}