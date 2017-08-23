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
    public class TrainingPlanController : Controller
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: Admin/TrainingPlan
        public ActionResult Index()
        {
            return View(db.TrainingPlans.ToList());
        }

        // GET: Admin/TrainingPlan/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingPlan trainingPlan = db.TrainingPlans.Find(id);
            if (trainingPlan == null)
            {
                return HttpNotFound();
            }
            return View(trainingPlan);
        }

        // GET: Admin/TrainingPlan/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/TrainingPlan/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] TrainingPlan trainingPlan)
        {
            if (ModelState.IsValid)
            {
                trainingPlan.Id = Guid.NewGuid();
                db.TrainingPlans.Add(trainingPlan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(trainingPlan);
        }

        // GET: Admin/TrainingPlan/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingPlan trainingPlan = db.TrainingPlans.Find(id);
            if (trainingPlan == null)
            {
                return HttpNotFound();
            }
            return View(trainingPlan);
        }

        // POST: Admin/TrainingPlan/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] TrainingPlan trainingPlan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainingPlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trainingPlan);
        }

        // GET: Admin/TrainingPlan/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingPlan trainingPlan = db.TrainingPlans.Find(id);
            if (trainingPlan == null)
            {
                return HttpNotFound();
            }
            return View(trainingPlan);
        }

        // POST: Admin/TrainingPlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TrainingPlan trainingPlan = db.TrainingPlans.Find(id);
            db.TrainingPlans.Remove(trainingPlan);
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
