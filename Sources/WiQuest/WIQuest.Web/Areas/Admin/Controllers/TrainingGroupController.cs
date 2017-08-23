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
    public class TrainingGroupController : Controller
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: Admin/TrainingGroup
        public ActionResult Index(Guid? id)
        {
            if (!id.HasValue)
                return View("List", db.TrainingGroups.ToList());

            var training = db.TrainingPlans.SingleOrDefault(x => x.Id == id.Value);

            return View(training);
        }

        // GET: Admin/TrainingGroup/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingGroup trainingGroup = db.TrainingGroups.Find(id);
            if (trainingGroup == null)
            {
                return HttpNotFound();
            }
            return View(trainingGroup);
        }

        // GET: Admin/TrainingGroup/Create
        public ActionResult Create(Guid? id)
        {
            var plan = db.TrainingPlans.SingleOrDefault(x => x.Id == id.Value);

            var model = new TrainingGroup();
            model.TrainingPlan = plan;


            return View(model);
        }

        // POST: Admin/TrainingGroup/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingGroup trainingGroup)
        {
            if (ModelState.IsValid)
            {
                var plan = db.TrainingPlans.SingleOrDefault(x => x.Id == trainingGroup.TrainingPlan.Id);

                if (plan != null)
                {
                    trainingGroup.Id = Guid.NewGuid();
                    trainingGroup.TrainingPlan = plan;
                    db.TrainingGroups.Add(trainingGroup);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = plan.Id });
                }
            }

            return View(trainingGroup);
        }

        // GET: Admin/TrainingGroup/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingGroup trainingGroup = db.TrainingGroups.Find(id);
            if (trainingGroup == null)
            {
                return HttpNotFound();
            }
            return View(trainingGroup);
        }

        // POST: Admin/TrainingGroup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,IsPublic,AccessToken")] TrainingGroup trainingGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainingGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trainingGroup);
        }

        // GET: Admin/TrainingGroup/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingGroup trainingGroup = db.TrainingGroups.Find(id);
            if (trainingGroup == null)
            {
                return HttpNotFound();
            }
            return View(trainingGroup);
        }

        // POST: Admin/TrainingGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TrainingGroup trainingGroup = db.TrainingGroups.Find(id);
            db.TrainingGroups.Remove(trainingGroup);
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
