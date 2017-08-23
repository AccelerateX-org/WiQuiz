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
    public class QuizQuestionsController : Controller
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: Admin/QuizQuestions
        public ActionResult Index(Guid? id)
        {
            var section = db.QuizSections.SingleOrDefault(x => x.Id == id.Value);

            return View(section);
        }

        // GET: Admin/QuizQuestions/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizQuestion quizQuestion = db.QuizQuestions.Find(id);
            if (quizQuestion == null)
            {
                return HttpNotFound();
            }
            return View(quizQuestion);
        }

        // GET: Admin/QuizQuestions/Create
        public ActionResult Create(Guid? id)
        {
            var section = db.QuizSections.SingleOrDefault(x => x.Id == id.Value);

            var model = new QuizQuestion();
            model.Section = section;

            var query = from c in db.Questions
                        select c;

            ViewBag.Question = new SelectList(query, "Id", "Text");

            return View(model);
        }

        // POST: Admin/QuizQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuizQuestion quizQuestion)
        {
            if (ModelState.IsValid)
            {
                var section = db.QuizSections.SingleOrDefault(x => x.Id == quizQuestion.Section.Id);
                var question = db.Questions.SingleOrDefault(x => x.Id == quizQuestion.Question.Id);

                if (section != null && question != null)
                {

                    quizQuestion.Id = Guid.NewGuid();
                    quizQuestion.Section = section;
                    quizQuestion.Question = question;
                    db.QuizQuestions.Add(quizQuestion);
                    db.SaveChanges();
                    return RedirectToAction("Index", new {id = section.Id});
                }
            }

            return View(quizQuestion);
        }

        // GET: Admin/QuizQuestions/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizQuestion quizQuestion = db.QuizQuestions.Find(id);
            if (quizQuestion == null)
            {
                return HttpNotFound();
            }
            return View(quizQuestion);
        }

        // POST: Admin/QuizQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Order")] QuizQuestion quizQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quizQuestion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(quizQuestion);
        }

        // GET: Admin/QuizQuestions/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizQuestion quizQuestion = db.QuizQuestions.Find(id);
            if (quizQuestion == null)
            {
                return HttpNotFound();
            }
            return View(quizQuestion);
        }

        // POST: Admin/QuizQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            QuizQuestion quizQuestion = db.QuizQuestions.Find(id);
            db.QuizQuestions.Remove(quizQuestion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        public ActionResult Select(Guid? id)
        {
            var model = db.QuizSections.SingleOrDefault(x => x.Id == id.Value);

            return View(model);
        }

        [HttpPost]
        public PartialViewResult Search(string searchString)
        {
            var questions = db.Questions.Where(x => x.Tags.Any(t => t.Name.ToLower().Contains(searchString.ToLower()))).ToList();

            return PartialView("_QuestionList", questions);
        }

        [HttpPost]
        public EmptyResult AddQuestions(Guid sectionId, ICollection<Guid> questionIds)
        {
            var section = db.QuizSections.SingleOrDefault(x => x.Id == sectionId);

            if (section != null && questionIds != null)
            {
                foreach (var questionId in questionIds)
                {
                    if (section.Questions.All(q => q.Question.Id != questionId))
                    {
                        var question = db.Questions.SingleOrDefault(x => x.Id == questionId);

                        var quizQuestion = new QuizQuestion
                        {
                            Id = Guid.NewGuid(),
                            Section = section,
                            Question = question
                        };
                        db.QuizQuestions.Add(quizQuestion);
                    }
                }
                db.SaveChanges();
            }

            return new EmptyResult();
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
