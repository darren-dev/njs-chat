using System;
using System.Collections.Generic;
using System.Web;
using NJS_Chat.Models;

namespace NJS_Chat.Helpers
{
    public class MessageHelper
    {
        internal static void SendMessage(IndexViewModel ivm, string username)
        {
            Global.Message bMessage = new Global.Message
            {
                DateSent = DateTime.UtcNow,
                From = username,
                To = ivm.To,
                MessageBody = ivm.Message.Trim(),
                MessageColor = Global.MessageColor.Green
            };


            HttpContext.Current.Session["_LastMessageTime"] = DateTime.UtcNow;
            MessageHelper mh = new MessageHelper();
            mh.QueMessage(bMessage);
        }

        internal void QueMessage(Global.Message message)
        {
            AssertMessageQueNotNull();

            AddToMessageQue(message);
        }

        internal List<Global.Message> GetMessages()
        {
            return GetMessageQue() ?? new List<Global.Message>();
        }

        private static void AddToMessageQue(Global.Message message)
        {
            lock (Global.Message.MessageQue)
            {
                var currMessageQue = GetMessageQue();
                if (currMessageQue == null) return;

                currMessageQue.Insert(0, message);
                Global.Message.MessageQue = currMessageQue;

            }
        }

        private static List<Global.Message> GetMessageQue()
        {
            AssertMessageQueNotNull();

            lock (Global.Message.MessageQue)
            {
                List<Global.Message> currMessageQue =
                    Global.Message.MessageQue;
                return currMessageQue;
            }
        }

        private static void AssertMessageQueNotNull()
        {
            if (Global.Message.MessageQue == null)
            {
                Global.Message.MessageQue = new List<Global.Message>();
            }
        }
    }
}