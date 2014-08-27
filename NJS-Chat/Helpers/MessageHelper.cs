using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NJS_Chat.Helpers
{
    public class MessageHelper
    {
        public static List<Message> MessageQue = new List<Message>();

        public void QueMessage(Message message)
        {
            if (MessageQue != null)
            {
                MessageQue.Insert(0, message);
            }
        }


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
    }
}