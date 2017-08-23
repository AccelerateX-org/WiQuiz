using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WIQuest.Web.API.Model;
using WIQuest.Web.Data;

namespace WIQuest.Web.API.Controllers
{
    public class QuizzesController : ApiController
    {
        private QuestDbContext db = new QuestDbContext();

        // GET: api/Quizzes
        public IQueryable<QuizViewModel> GetQuizzes()
        {
            var all = db.Quizzes;

            return all.Select(quiz => new QuizViewModel {Id = quiz.Id, Name = quiz.Name, Description = quiz.Description});
        }

        // GET: api/Quizzes/5
        [ResponseType(typeof(Quiz))]
        public IHttpActionResult GetQuiz(Guid id)
        {
            Quiz quiz = db.Quizzes.Find(id);
            if (quiz == null)
            {
                return NotFound();
            }

            return Ok(quiz);
        }

        // PUT: api/Quizzes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQuiz(Guid id, Quiz quiz)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != quiz.Id)
            {
                return BadRequest();
            }

            db.Entry(quiz).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuizExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Quizzes
        [ResponseType(typeof(Quiz))]
        public IHttpActionResult PostQuiz(Quiz quiz)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Quizzes.Add(quiz);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = quiz.Id }, quiz);
        }

        // DELETE: api/Quizzes/5
        [ResponseType(typeof(Quiz))]
        public IHttpActionResult DeleteQuiz(Guid id)
        {
            Quiz quiz = db.Quizzes.Find(id);
            if (quiz == null)
            {
                return NotFound();
            }

            db.Quizzes.Remove(quiz);
            db.SaveChanges();

            return Ok(quiz);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuizExists(Guid id)
        {
            return db.Quizzes.Count(e => e.Id == id) > 0;
        }
    }
}