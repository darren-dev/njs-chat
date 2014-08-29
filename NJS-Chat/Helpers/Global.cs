using System;

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
        }
    }
}