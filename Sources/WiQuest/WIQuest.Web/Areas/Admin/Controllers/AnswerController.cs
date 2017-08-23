using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WIQuest.Web.Data;
using WIQuest.Web.Models;

namespace WIQuest.Web.Areas.Admin.Controllers
{
    public class AnswerController : Controller
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: /Admin/Answer/
        public ActionResult Index(Guid? id)
        {
            var question = db.Questions.SingleOrDefault(x => x.Id == id.Value);

            return View(question);
        }

        // GET: /Admin/Answer/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionAnswer questionanswer = db.Answers.Find(id);
            if (questionanswer == null)
            {
                return HttpNotFound();
            }
            return View(questionanswer);
        }

        // GET: /Admin/Answer/Create
        public ActionResult Create(Guid id)
        {
            // die Id gehört zur Frage
            var question = db.Questions.SingleOrDefault(x => x.Id == id);

            var model = new QuestionAnswer();
            model.Question = question;
            return View(model);
        }

        // POST: /Admin/Answer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuestionAnswer questionanswer)
        {
            if (ModelState.IsValid)
            {
                var quiz = db.Questions.SingleOrDefault(x => x.Id == questionanswer.Question.Id);

                if (quiz != null)
                {
                    questionanswer.Id = Guid.NewGuid();
                    questionanswer.Question = quiz;
                    db.Answers.Add(questionanswer);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = quiz.Id });
                }

                return RedirectToAction("Index");

            }


            return View(questionanswer);
        }

        // GET: /Admin/Answer/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionAnswer questionanswer = db.Answers.Find(id);
            if (questionanswer == null)
            {
                return HttpNotFound();
            }
            return View(questionanswer);
        }

        // POST: /Admin/Answer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(QuestionAnswer questionanswer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questionanswer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new {id = questionanswer.Question.Id});
            }
            return View(questionanswer);
        }

        // GET: /Admin/Answer/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionAnswer questionanswer = db.Answers.Find(id);
            if (questionanswer == null)
            {
                return HttpNotFound();
            }
            return View(questionanswer);
        }

        // POST: /Admin/Answer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            QuestionAnswer questionanswer = db.Answers.Find(id);
            
            // die Question holen, solange sie da ist
            var qId = questionanswer.Question.Id;
            
            db.Answers.Remove(questionanswer);
            db.SaveChanges();
            
            return RedirectToAction("Index", new { id = qId });
        }

        public ActionResult UploadImage(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           QuestionAnswer answer = db.Answers.Find(id);
           if (answer == null)
            {
                return HttpNotFound();
            }
           return View(answer);
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase uploadFile, QuestionAnswer answer)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null)
                {
                    // Das Frageobjekt aus der Datenbank holen
                    var a = db.Answers.Find(answer.Id);

                    if (a != null)
                    {
                        // die BinaryStorage für die Datenbank anlegen
                        var image = new BinaryStorage()
                        {
                            ImageFileType = uploadFile.ContentType,
                            ImageData = new byte[uploadFile.ContentLength],
                        };

                        // Die Datei in die Binary Storage einlesen
                        uploadFile.InputStream.Read(image.ImageData, 0, uploadFile.ContentLength);

                        db.BinaryStorages.Add(image);
                        a.Image = image;

                        db.SaveChanges();

                        return RedirectToAction("Index", new {id = a.Question.Id});
                    }
                }
            }

            return View();
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
