using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using WIQuest.Web.API.Responses;
using WIQuest.Web.Controllers;
using WIQuest.Web.Data;
using WIQuest.Web.Services;

namespace WIQuest.Web.Areas.Admin.Controllers
{
    public class QuizGameController : BaseController
    {
        // GET: Admin/QuizGame
        public ActionResult Index()
        {
            var model = QuizDb.QuizGames.ToList();
            return View(model);
        }

        [ResponseType(typeof(WIQuest.Web.API.Responses.QuizGameResult))]
        public ActionResult GetQuizGameResult(Guid? gameId, Guid? playerId)
        {
            var result = new QuizGameResult();

            if (gameId.HasValue && playerId.HasValue)
            {

                var game = QuizDb.QuizGames.SingleOrDefault(x => x.Id == gameId.Value);
                var player = QuizDb.GamePlayers.SingleOrDefault(x => x.Id == playerId.Value);

                // Die Antwort

                if (game != null && player != null)
                {
                    result.GameId = game.Id;
                    result.PlayerId = player.Id;
                    result.QuizId = game.Levels.First().Quiz.Id;

                    var service = new GameStatisticsService(result.QuizId, player.UserId);

                    var statistics = service.GetStatistics(game.Id);
                    result.QuestionCount = statistics.TotalQuestionCount;
                    result.CorrectAnswerCount = statistics.CorrectAnswerCount;
                    result.PersonalRank = service.GetPersonalRank(result.CorrectAnswerCount);
                    result.TotalRank = service.GetTotalRank(result.CorrectAnswerCount);

                    result.Message = "OK";
                }
                else
                {
                    var sb = new StringBuilder();
                    if (game == null)
                    {
                        sb.Append("Spiel fehlt");
                    }
                    if (player == null)
                    {
                        sb.Append("Spieler fehlt");
                    }
                    result.Message = sb.ToString();
                }
            }
            else
            {
                result.Message = "Falsche Parameter";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}