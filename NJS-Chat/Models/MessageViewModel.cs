using System.Collections.Generic;
using NJS_Chat.Helpers;

namespace NJS_Chat.Models
{
    public class MessageViewModel
    {
        public List<Global.Message> Messages { get; set; }
        public string CurrentUser { get; set; }
    }
}