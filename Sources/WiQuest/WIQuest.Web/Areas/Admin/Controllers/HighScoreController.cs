using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WIQuest.Web.Areas.Admin.Models;
using WIQuest.Web.Controllers;

namespace WIQuest.Web.Areas.Admin.Controllers
{
    public class HighScoreController : BaseController
    {
        // GET: Admin/HighScore
        public ActionResult Index()
        {
            var model = new List<HighScoreViewModel>();

            var allQuizzes = QuizDb.Quizzes.ToList();

            foreach (var quiz in allQuizzes)
            {
                var gameModel = new HighScoreViewModel();

                gameModel.Quiz = quiz;

                gameModel.Games = QuizDb.QuizGames.Where(x => x.Levels.Any(y => y.Quiz.Id == quiz.Id)).ToList(); ;

                gameModel.Players = QuizDb.GamePlayers.Where(x => x.Game.Levels.Any(y => y.Quiz.Id == quiz.Id)).Select(x => x.UserId).Distinct().ToList();

                gameModel.Logs = QuizDb.GameLogs.Where(x => x.Game.Levels.Any(y => y.Quiz.Id == quiz.Id)).ToList();
                
                model.Add(gameModel);
            }
            
            
            
            return View(model);
        }

        public ActionResult ClearLog(Guid id)
        {
            var allGames = QuizDb.QuizGames.Where(x => x.Levels.Any(y => y.Quiz.Id == id)).ToList(); ;

            foreach (var game in allGames)
            {
                foreach (var level in game.Levels.ToList())
                {
                    QuizDb.GameLevels.Remove(level);
                }

                foreach (var player in game.Players.ToList())
                {
                    QuizDb.GamePlayers.Remove(player);
                }
            }

            var allLogs = QuizDb.GameLogs.Where(x => x.Game.Levels.Any(y => y.Quiz.Id == id)).ToList();

            foreach (var log in allLogs)
            {
                QuizDb.GameLogs.Remove(log);
            }

            QuizDb.SaveChanges();

            return RedirectToAction("Index");
        }


        public ActionResult Statistics()
        {
            return View();
        }
    }
}