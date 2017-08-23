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
    public class QuizSectionsController : Controller
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: Admin/QuizSections
        public ActionResult Index(Guid? id)
        {
            if (!id.HasValue)
                return View("List", db.QuizSections.ToList());

            var quiz = db.Quizzes.SingleOrDefault(x => x.Id == id.Value);

            return View(quiz);
        }

        // GET: Admin/QuizSections/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizSection quizSection = db.QuizSections.Find(id);
            if (quizSection == null)
            {
                return HttpNotFound();
            }
            return View(quizSection);
        }

        // GET: Admin/QuizSections/Create
        public ActionResult Create(Guid? id)
        {
            var quiz = db.Quizzes.SingleOrDefault(x => x.Id == id.Value);

            var model = new QuizSection();
            model.Quiz = quiz;

            return View(model);
        }

        // POST: Admin/QuizSections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuizSection quizSection)
        {
            if (ModelState.IsValid)
            {
                var quiz = db.Quizzes.SingleOrDefault(x => x.Id == quizSection.Quiz.Id);

                if (quiz != null)
                {
                    quizSection.Id = Guid.NewGuid();
                    quizSection.Quiz = quiz;
                    db.QuizSections.Add(quizSection);
                    db.SaveChanges();
                    return RedirectToAction("Index", new {id = quiz.Id});
                }

                return RedirectToAction("Index");
            }

            return View(quizSection);
        }

        // GET: Admin/QuizSections/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizSection quizSection = db.QuizSections.Find(id);
            if (quizSection == null)
            {
                return HttpNotFound();
            }
            return View(quizSection);
        }

        // POST: Admin/QuizSections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] QuizSection quizSection)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quizSection).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(quizSection);
        }

        // GET: Admin/QuizSections/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizSection quizSection = db.QuizSections.Find(id);
            if (quizSection == null)
            {
                return HttpNotFound();
            }
            return View(quizSection);
        }

        // POST: Admin/QuizSections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            QuizSection quizSection = db.QuizSections.Find(id);
            db.QuizSections.Remove(quizSection);
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
