using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prepaid.Models
{
    public class AdminSession
    {
        public string UUID { get; set; }

        public int RoleID { get; set; }

        public string UserName { get; set; }

        public string RealName { get; set; }

        public string Phone { get; set; }
    }
}