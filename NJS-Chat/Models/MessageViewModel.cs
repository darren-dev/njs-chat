using System.Collections.Generic;
using NJS_Chat.Helpers;

namespace NJS_Chat.Models
{
    public class MessageViewModel
    {
        internal List<Global.Message> Messages { get; set; }
        internal string CurrentUser { get; set; }
    }
}