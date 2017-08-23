using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WIQuest.Web.Areas.Quiz.Models;
using WIQuest.Web.Controllers;
using WIQuest.Web.Services;

namespace WIQuest.Web.Areas.Quiz.Controllers
{
    public class StatisticsController : BaseController
    {
        // GET: Quiz/Statistics
        public ActionResult Personal(Guid? id)
        {
            var quiz = GetQuiz(id);
            var user = GetCurrentUser();

            var service = new GameStatisticsService(quiz.Id, user.Id);

            var myGames = service.GetMyGames();

            var model = new QuizGameStatisticsViewModel();
            model.Quiz = quiz;

            foreach (var game in myGames)
            {
                model.Games.Add(service.GetStatistics(game.Id));
            }


            return View(model);
        }

        public ActionResult Total(Guid? id)
        {
            var quiz = GetQuiz(id);
            var user = GetCurrentUser();

            var service = new GameStatisticsService(quiz.Id, user.Id);

            var myGames = service.GetTotalGames();

            var model = new QuizGameStatisticsViewModel();
            model.Quiz = quiz;

            foreach (var game in myGames)
            {
                if (game.Players.Any())
                {
                    var stat = service.GetStatistics(game.Id, game.Players.First().UserId);
                    if (stat != null)
                    {
                        model.Games.Add(stat);
                    }
                }
            }


            return View(model);
        }
    }
}