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
    public class QuestionController : Controller
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: /Admin/Question/
        public ActionResult Index()
        {
            /*
            var model = new List<Question>();
            return View(model);
             */

            return View("DataTable", db.Questions.ToList());
        }

        [HttpPost]
        public PartialViewResult List()
        {
            return PartialView("_QuestionList", db.Questions.ToList());
        }

        // GET: /Admin/Question/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: /Admin/Question/Create
        public ActionResult Create()
        {
            // INhalt aufbereiten
            // Liste aller Kategorien
            ViewBag.Category = new SelectList(db.Categories.ToList());

            // An Stelle der Strings wird die SelectList aus den Kategorien aus der Datenbank aufgebaut
            // db.Categories.ToList()


            return View();
        }

        // POST: /Admin/Question/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Question question)
        {
            if (ModelState.IsValid)
            {
                question.Id = Guid.NewGuid();
                db.Questions.Add(question);
                db.SaveChanges();
                
                
                return RedirectToAction("Details", new {id = question.Id});
            }

            return View(question);
        }

        // GET: /Admin/Question/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: /Admin/Question/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = question.Id });
            }
            return View(question);
        }

        // GET: /Admin/Question/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: /Admin/Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult UploadImage(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase uploadFile, Question question)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null)
                {
                    // Das Frageobjekt aus der Datenbank holen
                    var q = db.Questions.Find(question.Id);

                    if (q!= null)
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
                        q.Image = image;

                        db.SaveChanges();

                        return RedirectToAction("Details", new {id = q.Id});
                    }
                }
            }

            return View();
        }

        public JsonResult Download(Guid? id)
        {
            /*
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
             */
            Question question = db.Questions.Find(id);
            /*
            if (question == null)
            {
                return HttpNotFound();
            }
             */

            var qm = new QuestionModel
            {
                Title = question.Title,
                Text = question.Text,
                Topic = question.Category.Name
            };


            foreach (var answer in question.Answers)
            {
                qm.Answers.Add(new QuestionAnswerModel()
                {
                    Text = answer.Text,
                    Order = answer.Reihenfolge,
                    IsCorrect = answer.IsCorrect
                });
            }

            qm.Tags.Add("Beispiel-Tag 1");
            qm.Tags.Add("Beispiel-Tag 2");

            var model = new List<QuestionModel>();
            model.Add(qm);

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public PartialViewResult Search(string searchString)
        {
            var questions = db.Questions.Where(x => x.Tags.Any(t => t.Name.ToLower().Contains(searchString.ToLower()))).ToList();
            
            return PartialView("_QuestionList", questions);
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
