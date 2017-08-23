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
    public class TrainingExerciseController : Controller
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: Admin/TrainingExercise
        public ActionResult Index(Guid? id)
        {
            if (!id.HasValue)
                return View("List", db.TrainingExercises.ToList());

            var training = db.TrainingSessions.SingleOrDefault(x => x.Id == id.Value);

            return View(training);
        }

        // GET: Admin/TrainingExercise/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingExercise trainingExercise = db.TrainingExercises.Find(id);
            if (trainingExercise == null)
            {
                return HttpNotFound();
            }
            return View(trainingExercise);
        }

        // GET: Admin/TrainingExercise/Create
        public ActionResult Create(Guid? id)
        {
            var training = db.TrainingSessions.SingleOrDefault(x => x.Id == id.Value);

            var model = new TrainingExercise();
            model.TrainingSession = training;


            return View(model);
        }

        // POST: Admin/TrainingExercise/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingExercise trainingExercise)
        {
            if (ModelState.IsValid)
            {
                var session = db.TrainingSessions.SingleOrDefault(x => x.Id == trainingExercise.TrainingSession.Id);
                var quiz = db.Quizzes.SingleOrDefault(x => x.Id == trainingExercise.Quiz.Id);

                if (session != null)
                {
                    trainingExercise.Id = Guid.NewGuid();
                    trainingExercise.TrainingSession = session;
                    trainingExercise.Quiz = quiz;
                    db.TrainingExercises.Add(trainingExercise);
                    db.SaveChanges();
                    return RedirectToAction("Index", new {id = session.Id});
                }
            }

            return View(trainingExercise);
        }

        // GET: Admin/TrainingExercise/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingExercise trainingExercise = db.TrainingExercises.Find(id);
            if (trainingExercise == null)
            {
                return HttpNotFound();
            }
            return View(trainingExercise);
        }

        // POST: Admin/TrainingExercise/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TrainingExercise trainingExercise)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainingExercise).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new {id=trainingExercise.TrainingSession.Id});
            }
            return View(trainingExercise);
        }

        // GET: Admin/TrainingExercise/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingExercise trainingExercise = db.TrainingExercises.Find(id);
            if (trainingExercise == null)
            {
                return HttpNotFound();
            }
            return View(trainingExercise);
        }

        // POST: Admin/TrainingExercise/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TrainingExercise trainingExercise = db.TrainingExercises.Find(id);
            db.TrainingExercises.Remove(trainingExercise);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult TurnOn(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingExercise trainingExercise = db.TrainingExercises.Find(id);
            if (trainingExercise == null)
            {
                return HttpNotFound();
            }

            trainingExercise.IsAvailabe = true;
            db.SaveChanges();

            return RedirectToAction("Index", new { id = trainingExercise.TrainingSession.Id });
        }

        public ActionResult TurnOff(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingExercise trainingExercise = db.TrainingExercises.Find(id);
            if (trainingExercise == null)
            {
                return HttpNotFound();
            }

            trainingExercise.IsAvailabe = false;
            db.SaveChanges();

            return RedirectToAction("Index", new { id = trainingExercise.TrainingSession.Id });
        }



        [HttpPost]
        public JsonResult QuizList(string name)
        {
            var allTags = db.Quizzes.Where(x => x.Name.Contains(name))
                .OrderBy(x => x.Name)
                .Select(l => new
                {
                    name = l.Name,
                    id = l.Id,
                })
                .Take(10);

            return Json(allTags);
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
