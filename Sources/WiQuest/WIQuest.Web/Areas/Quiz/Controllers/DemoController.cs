using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Pitchfork.QRGenerator;
using WIQuest.Web.Areas.Quiz.Models;
using WIQuest.Web.Controllers;
using WIQuest.Web.Data;
using WIQuest.Web.Models;
using WIQuest.Web.Services;
using WIQuest.Web.Utils;

namespace WIQuest.Web.Areas.Quiz.Controllers
{
    [AllowAnonymous]
    public class DemoController : BaseController
    {
        // GET: Quiz/Demo
        public ActionResult Index()
        {
            var quizzes = QuizDb.Quizzes.Where(x => x.IsPubslished).ToList();

            return View(quizzes);
        }

        public ActionResult Launch(Guid id)
        {
            var model = QuizDb.Quizzes.SingleOrDefault(x => x.Id == id);

            return View(model);
        }


        public ActionResult StartGame(Guid id)
        {
            // User identifizieren
            // den gibt es nicht => dummy user!
            var user = GetAnoynmousUser();

            // Spiel anlegen
            var gameService = new GamePlayService();
            var game = gameService.CreateSingleUserGame(user.Id, id);

            var gameState = new GamePlayState();
            gameState.Game = game;
            gameState.Player = game.Players.First();
            gameState.Quiz = game.Levels.First().Quiz;

            foreach (var quizSection in gameState.Quiz.Sections)
            {
                foreach (var quizQuestion in quizSection.Questions)
                {
                    gameState.QuestionsLeft.Add(quizQuestion.Question);
                }
            }

            // Fragen mischen
            gameState.QuestionsLeft.Shuffle();


            // Statistik erstellen
            gameState.QuestionTotalCount = gameState.QuestionsLeft.Count;

            // In einer Session sichern
            // oder in DB?
            Session["GameState"] = gameState;


            var question = gameState.QuestionsLeft.FirstOrDefault();

            var model = new QuestionViewModel();
            model.Question = question;
            model.Answers = question.Answers.ToList();
            model.Answers.Shuffle();
            model.Total = gameState.QuestionTotalCount;
            model.Index = gameState.QuestionTotalCount - gameState.QuestionsLeft.Count + 1;

            gameState.CurrentQuestion = model;

            //return View("PlayGameSPA", model);
            return View("PlayGame", model);
        }

        private ApplicationUser GetAnoynmousUser()
        {
            var user = UserManager.FindByName("Anonymous");

            if (user == null)
            {
                user = new ApplicationUser { UserName = "Anonymous", Email = "anonymous@wiquiz.org" };
                UserManager.Create(user, "Pas1234?");
                user = UserManager.FindByName("Anonymous");
            }

            return user;
        }

        [HttpPost]
        public PartialViewResult Answer(Guid questionId, Guid answerId)
        {
            var gameState = Session["GameState"] as GamePlayState;

            var answer = QuizDb.Answers.SingleOrDefault(x => x.Id == answerId);
            var game = QuizDb.QuizGames.SingleOrDefault(x => x.Id == gameState.Game.Id);
            var player = QuizDb.GamePlayers.SingleOrDefault(x => x.Id == gameState.Player.Id);

            var log = new GameLog();
            log.Answer = answer;
            log.Game = game;
            log.Player = player;
            log.LogDateTime = DateTime.Now;

            QuizDb.GameLogs.Add(log);
            QuizDb.SaveChanges();

            // Update
            var statService = new StatisticsService(QuizDb);
            statService.NotifyUpdates();

            var questionToDelete = gameState.QuestionsLeft.SingleOrDefault(x => x.Id == answer.Question.Id);

            gameState.QuestionsLeft.Remove(questionToDelete);

            // jetzt die Antwort mit Ergebnis zurückschicken


            // die Frage
            // die Antwort
            var model = new QuestionAnswerViewModel();
            model.Answers = gameState.CurrentQuestion.Answers;
            model.GivenAnswer = answer;
            model.CorrectAnswer = questionToDelete.Answers.FirstOrDefault(x => x.IsCorrect);

            return PartialView("_Answer", model);
        }

        [HttpPost]
        public PartialViewResult NextQuestion()
        {
            var gameState = Session["GameState"] as GamePlayState;


            var question = gameState.QuestionsLeft.FirstOrDefault();

            if (question != null)
            {
                var model = new QuestionViewModel();
                model.Question = question;
                model.Answers = question.Answers.ToList();
                model.Answers.Shuffle();
                model.Total = gameState.QuestionTotalCount;
                model.Index = gameState.QuestionTotalCount - gameState.QuestionsLeft.Count + 1;

                gameState.CurrentQuestion = model;

                return PartialView("_Question", model);
            }


            var game = QuizDb.QuizGames.SingleOrDefault(x => x.Id == gameState.Game.Id);
            var player = QuizDb.GamePlayers.SingleOrDefault(x => x.Id == gameState.Player.Id);

            var statisticsService = new GameStatisticsService(gameState.Quiz.Id, player.UserId);

            var statistics = statisticsService.GetStatistics(game.Id);

            return PartialView("_EndGame", statistics);
        }


        public ActionResult GetQRCode(Guid? id)
        {
            var quiz = QuizDb.Quizzes.SingleOrDefault(q => q.Id == id);

            if (quiz != null)
            {
                var url = Url.Action("StartGame", "Demo", new { id = quiz.Id }, Request.Url.Scheme);

                var img = Pitchfork.QRGenerator.QRCodeImageGenerator.GetQRCode(url, ErrorCorrectionLevel.Q);

                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                return File(ms.ToArray(), "image/png");
            }

            return File("~/images/48px-Face-sad_svg.png", "image/png");
        }
    }
}