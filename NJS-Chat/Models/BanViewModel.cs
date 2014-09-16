using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NJS_Chat.Models
{
    public class BanViewModel
    {
        public string UserName { get; set; }
        public string BanReason { get; set; }
        public DateTime? BannedDate { get; set; }
        public DateTime? BanLiftDate { get; set; }
        public bool? IsBanned { get; set; }
    }
}