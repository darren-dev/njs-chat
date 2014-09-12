using System.Collections.Generic;
using System.Web.Mvc;

namespace NJS_Chat.Models
{
    public class IndexViewModel
    {
        [AllowHtml]
        public string Message { get; set; }
        public string To { get; set; }
        public IEnumerable<SelectListItem> ToSelectItems { get; set; } 
    }
}