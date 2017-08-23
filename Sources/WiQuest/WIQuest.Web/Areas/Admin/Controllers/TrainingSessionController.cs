using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Admin.Controllers
{
    public class TrainingSessionController : Controller
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: Admin/TrainingSession
        public ActionResult Index(Guid? id)
        {
            if (!id.HasValue)
                return View("List", db.TrainingSessions.ToList());

            var training = db.TrainingPlans.SingleOrDefault(x => x.Id == id.Value);

            return View(training);
        }

        // GET: Admin/TrainingSession/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingSession trainingSession = db.TrainingSessions.Find(id);
            if (trainingSession == null)
            {
                return HttpNotFound();
            }
            return View(trainingSession);
        }

        // GET: Admin/TrainingSession/Create
        public ActionResult Create(Guid? id)
        {
            var plan = db.TrainingPlans.SingleOrDefault(x => x.Id == id.Value);

            var model = new TrainingSession();
            model.TrainingPlan = plan;


            return View(model);
        }

        // POST: Admin/TrainingSession/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingSession trainingSession)
        {
            if (ModelState.IsValid)
            {
                var plan = db.TrainingPlans.SingleOrDefault(x => x.Id == trainingSession.TrainingPlan.Id);

                if (plan != null)
                {

                    trainingSession.Id = Guid.NewGuid();
                    trainingSession.TrainingPlan = plan;
                    db.TrainingSessions.Add(trainingSession);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = plan.Id });
                }

            }

            return View(trainingSession);
        }

        // GET: Admin/TrainingSession/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingSession trainingSession = db.TrainingSessions.Find(id);
            if (trainingSession == null)
            {
                return HttpNotFound();
            }
            return View(trainingSession);
        }

        // POST: Admin/TrainingSession/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TrainingSession trainingSession)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainingSession).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new {id=trainingSession.TrainingPlan.Id});
            }
            return View(trainingSession);
        }

        // GET: Admin/TrainingSession/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingSession trainingSession = db.TrainingSessions.Find(id);
            if (trainingSession == null)
            {
                return HttpNotFound();
            }
            return View(trainingSession);
        }

        // POST: Admin/TrainingSession/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TrainingSession trainingSession = db.TrainingSessions.Find(id);
            db.TrainingSessions.Remove(trainingSession);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
