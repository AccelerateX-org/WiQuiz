using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Admin.Controllers
{
    public class QuestionTagController : Controller
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: /Admin/Answer/
        public ActionResult Index(Guid? id)
        {
            var question = db.Questions.SingleOrDefault(x => x.Id == id.Value);

            return View(question);
        }


        [HttpPost]
        public PartialViewResult SendTag(Guid id, string tagName)
        {
            var question = db.Questions.SingleOrDefault(x => x.Id == id);

            if (question != null)
            {
                // Gibt es den Tag schon bei dieser Frage?
                var tagExists = question.Tags.Any(x => x.Name.ToLower().Equals(tagName.ToLower()));

                if (!tagExists)
                {
                    // gibt es den Tag generell schon?
                    var tag = db.QuestionTags.FirstOrDefault(x => x.Name.ToLower().Equals(tagName.ToLower()));

                    if (tag == null)
                    {
                        tag = new QuestionTag();
                        tag.Name = tagName;
                        db.QuestionTags.Add(tag);
                        db.SaveChanges();
                    }

                    question.Tags.Add(tag);
                    // braucht es nur einmal, oder? testen
                    db.SaveChanges();
                }
            }

            return PartialView("_TagList", question);
        }


        [HttpPost]
        public PartialViewResult DeleteTag(Guid? questionId, Guid? tagId)
        {
            var question = db.Questions.SingleOrDefault(x => x.Id == questionId);
            var tag = db.QuestionTags.SingleOrDefault(x => x.Id == tagId);

            if (question != null && tag != null && question.Tags.Contains(tag))
            {
                question.Tags.Remove(tag);
                db.SaveChanges();
            }

            return PartialView("_TagList", question);
        }

        [HttpPost]
        public JsonResult TagList(string name)
        {
            var allTags = db.QuestionTags.Where(x => x.Name.Contains(name))
                .OrderBy(x => x.Name)
                .Select(l => new
                {
                    name = l.Name,
                    id = l.Id,
                })
                .Take(10);

            return Json(allTags);
        }



    }
}