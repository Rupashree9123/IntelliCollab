using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelliCollabUI.Models
{
    public class AccessRole
    {
        public decimal AccessId { get; set; }
        public string Controller { get; set; }
        public string Actions { get; set; }
        public string Active { get; set; }
        public string ScreenInfo { get; set; }
        public string RoleType { get; set; }
    }
}