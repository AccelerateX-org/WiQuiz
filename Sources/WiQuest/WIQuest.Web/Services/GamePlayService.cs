using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIQuest.Web.Data;

namespace WIQuest.Web.Services
{
    public class GamePlayService
    {
        private readonly QuestDbContext _quizDb;

        public GamePlayService()
        {
            _quizDb = new QuestDbContext();
        }

        public GamePlayService(QuestDbContext db)
        {
            _quizDb = db;
        }


        public Data.QuizGame CreateSingleUserGame(string userId, Guid quizId)
        {
            var quiz = _quizDb.Quizzes.FirstOrDefault(x => x.Id == quizId);

            if (quiz == null)
                return null;

            var level = new GameLevel
            {
                Quiz =  quiz
            };

            var player = new GamePlayer
            {
                UserId = userId,
                IsInitiator = true
            };
            

            var game = new QuizGame
            {
                CreatedAt = DateTime.Now,
                Levels = new List<GameLevel>
                {
                    level
                },
                Players = new List<GamePlayer>
                {
                    player
                }
            };

            try
            {
                //quizDb.GameLevels.Add(level);
                //quizDb.GamePlayers.Add(player);
                _quizDb.QuizGames.Add(game);
                _quizDb.SaveChanges();

                return game;

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}