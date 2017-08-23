using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using WIQuest.Web.API.Responses;
using WIQuest.Web.Areas.Quiz.Models;
using WIQuest.Web.Data;
using WIQuest.Web.Services;

namespace WIQuest.Web.API.Controllers
{
    public class GameController : ApiController
    {
        private QuestDbContext quizDb = new QuestDbContext();

        [ResponseType(typeof (GameList))]
        public IHttpActionResult GetGameList(string userId)
        {
            var model = new GameList();
            model.Games = new List<GameSummary>();

            var quizzes = quizDb.Quizzes.Where(x => x.IsPubslished).ToList();

            foreach (var quiz in quizzes)
            {
                var statisticsService = new GameStatisticsService(quiz.Id, userId);

                // TODO: Liste auswerten
                var gameModel = new GameSummary();

                gameModel.Name = quiz.Name;
                gameModel.QuizId = quiz.Id;
                gameModel.QuestionCount = statisticsService.GetQuestions().Count;
                gameModel.MyGameCount = statisticsService.GetMyGames().Count;           // wie oft habe ich gespielt
                gameModel.TotalGameCount = statisticsService.GetTotalGames().Count;

                var bestGame = statisticsService.GetBestGame();
                if (bestGame != null)
                {
                    gameModel.BestGame = new GameApproach
                    {
                        PlayDateTime = bestGame.Game.CreatedAt,
                        Rank = 0,
                        Score = bestGame.CorrectAnswerCount
                    };
                }

                var lastGame = statisticsService.GetLastGame();
                if (lastGame != null)
                {
                    gameModel.LastGame = new GameApproach
                    {
                        PlayDateTime = lastGame.Game.CreatedAt,
                        Rank = 0,
                        Score = lastGame.CorrectAnswerCount
                    };
                }

                model.Games.Add(gameModel);
            }
            


            return Ok(model);
        }

        [ResponseType(typeof(WIQuest.Web.API.Responses.QuizGameCreate))]
        public IHttpActionResult GetStartQuizGame(string userId, string quizId)
        {
            var quizGame = new WIQuest.Web.API.Responses.QuizGameCreate();

            try
            {
                var gameService = new GamePlayService();
                var game = gameService.CreateSingleUserGame(userId, Guid.Parse(quizId));

                if (game != null)
                {
                    quizGame.GameId = game.Id;
                    quizGame.PlayerId = game.Players.First().Id;
                    quizGame.Quiz = game.Levels.First().Quiz;
                }
                else
                {
                    quizGame.Message = "Quiz existiert nicht!";
                }
            }
            catch (Exception e)
            {
                quizGame.Message = e.Message;
            }

            quizGame.Message = "Test";

            return Ok(quizGame);
        }

        [ResponseType(typeof(WIQuest.Web.API.Responses.QuizGameAnswerResult))]
        public IHttpActionResult GetLogAnswer(Guid? gameId, Guid? playerId, Guid? answerId)
        {
            var QuizDb = new QuestDbContext();

            var result = new QuizGameAnswerResult();

            if (gameId.HasValue && playerId.HasValue && answerId.HasValue)
            {

                var answer = QuizDb.Answers.SingleOrDefault(x => x.Id == answerId.Value);
                var game = QuizDb.QuizGames.SingleOrDefault(x => x.Id == gameId.Value);
                var player = QuizDb.GamePlayers.SingleOrDefault(x => x.Id == playerId.Value);

                // Die Antwort

                if (answer != null && game != null && player != null)
                {
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


                    result.QuizId = game.Levels.First().Quiz.Id;
                    result.AnswerId = answer.Id;
                    result.IsCorrect = answer.IsCorrect;
                    if (answer.IsCorrect)
                    {
                        result.Score = 1;
                    }

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
                    if (answer == null)
                    {
                        sb.Append("Antwort fehlt");
                    }
                    result.Message = sb.ToString();
                }
            }
            else
            {
                result.Message = "Falsche Parameter";
            }

            return Ok(result);
        }




        [ResponseType(typeof(WIQuest.Web.API.Responses.QuizGameResult))]
        public IHttpActionResult GetQuizGameResult(Guid? gameId, Guid? playerId)
        {
            var QuizDb = new QuestDbContext();

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

            return Ok(result);
        }

    }
}
