using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Admin.Controllers
{
    public class TrainingAnalysisController : Controller
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: Admin/TrainingAnalyses
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Exercise(Guid exId)
        {
            var exercise = db.TrainingExercises.SingleOrDefault(x => x.Id == exId);



            return View(exercise);
        }
    }
}