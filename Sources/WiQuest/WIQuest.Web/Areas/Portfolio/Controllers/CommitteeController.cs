using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Portfolio.Controllers
{
    [AllowAnonymous]
    public class CommitteeController : Controller
    {
        // GET: Portfolio/Committee
        public ActionResult Index()
        {
            var db = new PortfolioDbContext();

            var model = db.Applications.ToList();

            return View(model);
        }
    }
}