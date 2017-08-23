using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIQuest.Web.Data;

namespace WIQuest.Web.Services
{
    public class GameStatisticsService
    {
        private QuestDbContext quizDb = new QuestDbContext();

        private List<QuizGame> games;
        private Quiz quiz;
        private string _userId;

        public GameStatisticsService(Guid quizId, string userId)
        {
            quiz = quizDb.Quizzes.SingleOrDefault(x => x.Id == quizId);
            this._userId = userId;

            games = quizDb.QuizGames.Where(x => x.Levels.Any(y => y.Quiz.Id == quiz.Id) &&
                                                x.Players.Any(p => p.UserId.Equals(userId))).ToList();

        }

        public List<Question> GetQuestions()
        {
            var list = new List<Question>();

            foreach (var quizSection in quiz.Sections)
            {
                list.AddRange(quizSection.Questions.Select(quizQuestion => quizQuestion.Question));
            }

            return list;
        }

        public List<QuizGame> GetMyGames()
        {
            return games;
        }

        public List<QuizGame> GetTotalGames()
        {
            return quizDb.QuizGames.Where(x => x.Levels.Any(y => y.Quiz.Id == quiz.Id)).ToList();
        }

        public GameStatistics GetStatistics(Guid gameId)
        {
            return GetStatistics(gameId, _userId);
        }


        public GameStatistics GetStatistics(Guid gameId, string userId)
        {
            var game = quizDb.QuizGames.SingleOrDefault(x => x.Id == gameId);

            if (game == null)
                return null;


            var statistics = new GameStatistics();

            statistics.Game = game;
            statistics.TotalQuestionCount = GetQuestions().Count;
            statistics.CorrectAnswerCount = quizDb.GameLogs.Count(x => x.Game.Id == gameId &&
                                       x.Player.UserId.Equals(userId) &&
                                       x.Answer.IsCorrect);
            statistics.AnswerCount = quizDb.GameLogs.Count(x => x.Game.Id == gameId &&
                                       x.Player.UserId.Equals(userId));

            statistics.ItIsMe = userId.Equals(_userId);
            
            return statistics;
        }



        public GameStatistics GetLastGame()
        {
            var lastGame = games.OrderBy(x => x.CreatedAt).LastOrDefault();
            if (lastGame != null)
                return GetStatistics(lastGame.Id);
            return null;
        }

        public GameStatistics GetBestGame()
        {
            GameStatistics bestStatistics = null;

            foreach (var game in games)
            {
                var statistics = GetStatistics(game.Id);
                if (bestStatistics == null ||
                    bestStatistics.CorrectAnswerCount < statistics.CorrectAnswerCount)
                {
                    bestStatistics = statistics;
                }
            }

            return bestStatistics;
        }


        internal int GetPersonalRank(int score)
        {
            var myGames = GetMyGames();
            return GetRank(myGames, score);
        }

        internal int GetTotalRank(int score)
        {
            var allGames = GetTotalGames();
            return GetRank(allGames, score);
        }

        private int GetRank(List<QuizGame> gameList, int score)
        {
            var rankings = new SortedList<int, int>();
            foreach (var game in gameList)
            {
                var n = quizDb.GameLogs.Count(x => x.Game.Id == game.Id &&
                                       x.Player.UserId.Equals(_userId) &&
                                       x.Answer.IsCorrect);

                if (!rankings.ContainsKey(n))
                {
                    rankings.Add(n, 0);
                }
                rankings[n]++;
            }

            var rank = rankings.Where(x => x.Key > score).Sum(x => x.Value) + 1;

            return rank;
        }

    }

    public class GameStatistics
    {
        public QuizGame Game { get; set; }

        /// <summary>
        /// Anzahl der Fragen
        /// </summary>
        public int TotalQuestionCount { get; set; }

        /// <summary>
        /// Anzahl der korrekten Antworten
        /// </summary>
        public int CorrectAnswerCount { get; set; }

        /// <summary>
        /// Anzahl der gegebenen Antworten
        /// </summary>
        public int AnswerCount { get; set; }

        public double Percentage
        {
            get { return CorrectAnswerCount/(double) TotalQuestionCount; }
        }

        public bool ItIsMe { get; set; }
    }

}