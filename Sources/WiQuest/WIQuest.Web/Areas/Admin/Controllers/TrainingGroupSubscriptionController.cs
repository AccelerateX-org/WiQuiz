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
    public class TrainingGroupSubscriptionController : Controller
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: Admin/TrainingGroupSubscription
        public ActionResult Index(Guid? id)
        {
            if (!id.HasValue)
                return View("List", db.TrainingGroupSubscriptions.ToList());

            var group = db.TrainingGroups.SingleOrDefault(x => x.Id == id.Value);

            return View(group);
        }

        // GET: Admin/TrainingGroupSubscription/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingGroupSubscription trainingGroupSubscription = db.TrainingGroupSubscriptions.Find(id);
            if (trainingGroupSubscription == null)
            {
                return HttpNotFound();
            }
            return View(trainingGroupSubscription);
        }

        // GET: Admin/TrainingGroupSubscription/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/TrainingGroupSubscription/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Userid,TimeStamp")] TrainingGroupSubscription trainingGroupSubscription)
        {
            if (ModelState.IsValid)
            {
                trainingGroupSubscription.Id = Guid.NewGuid();
                db.TrainingGroupSubscriptions.Add(trainingGroupSubscription);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(trainingGroupSubscription);
        }

        // GET: Admin/TrainingGroupSubscription/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingGroupSubscription trainingGroupSubscription = db.TrainingGroupSubscriptions.Find(id);
            if (trainingGroupSubscription == null)
            {
                return HttpNotFound();
            }
            return View(trainingGroupSubscription);
        }

        // POST: Admin/TrainingGroupSubscription/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Userid,TimeStamp")] TrainingGroupSubscription trainingGroupSubscription)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainingGroupSubscription).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trainingGroupSubscription);
        }

        // GET: Admin/TrainingGroupSubscription/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingGroupSubscription trainingGroupSubscription = db.TrainingGroupSubscriptions.Find(id);
            if (trainingGroupSubscription == null)
            {
                return HttpNotFound();
            }
            return View(trainingGroupSubscription);
        }

        // POST: Admin/TrainingGroupSubscription/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TrainingGroupSubscription trainingGroupSubscription = db.TrainingGroupSubscriptions.Find(id);
            db.TrainingGroupSubscriptions.Remove(trainingGroupSubscription);
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
