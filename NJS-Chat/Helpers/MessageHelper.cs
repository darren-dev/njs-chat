using System.Collections.Generic;
using System.Web;

namespace NJS_Chat.Helpers
{
    public class MessageHelper
    {
        internal void QueMessage(Global.Message message)
        {
            AssertMessageQueNotNull();

            var currMessageQue = GetMessageQue();
            if (currMessageQue == null) return;
            
            AddToMessageQue(message, currMessageQue);
        }

        internal List<Global.Message> GetMessages()
        {
            AssertMessageQueNotNull();

            return GetMessageQue() ?? new List<Global.Message>();
        }

        private static void AddToMessageQue(Global.Message message, List<Global.Message> currMessageQue)
        {
            lock (HttpContext.Current.Application["MessageQue"])
            {
                lock (currMessageQue)
                {
                    currMessageQue.Insert(0, message);
                    HttpContext.Current.Application["MessageQue"] = currMessageQue;
                }
            }
        }

        private static List<Global.Message> GetMessageQue()
        {
            lock (HttpContext.Current.Application["MessageQue"])
            {
                List<Global.Message> currMessageQue =
                    HttpContext.Current.Application["MessageQue"] as List<Global.Message>;
                return currMessageQue;
            }
        }

        private static void AssertMessageQueNotNull()
        {
            if (HttpContext.Current.Application["MessageQue"] == null)
            {
                HttpContext.Current.Application["MessageQue"] = new List<Global.Message>();
            }
        }
    }
}