using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIQuest.Web.Models;

namespace WIQuest.Web.Areas.SysAdmin.Models
{
    public class UserViewModel
    {
        public ApplicationUser User { get; set; }

        public bool IsQuizAdmin { get; set; }

        public bool IsUserAdmin { get; set; }
    }
}