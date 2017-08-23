using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WIQuest.Web.Areas.SysAdmin.Controllers
{
    [Authorize(Roles = "UserAdmin")]
    public class HomeController : Controller
    {
        // GET: SysAdmin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}