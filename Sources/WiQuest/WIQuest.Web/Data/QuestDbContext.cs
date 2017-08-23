using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class QuestDbContext : DbContext
    {
        public QuestDbContext() : base("QuestDb")
        {
            
        }

        public IDbSet<User> Users { get; set; }

        #region Allgemeine Infrastruktur
        public IDbSet<BinaryStorage> BinaryStorages { get; set; }
        #endregion

        #region Quizes
        public IDbSet<Quiz> Quizzes { get; set; }

        public IDbSet<QuizSection> QuizSections { get; set; }

        public IDbSet<QuizQuestion> QuizQuestions { get; set; }

        #endregion

        #region Fragenkatalog
        public IDbSet<QuestionCategory> Categories { get; set; }

        public IDbSet<Question> Questions { get; set; }

        public IDbSet<QuestionAnswer> Answers { get; set; }

        public IDbSet<QuestionTag> QuestionTags { get; set; }
        #endregion

        #region Ausführungen
        public IDbSet<QuestLog> QuestLogs { get; set; }
        #endregion

        #region Spiel
        public IDbSet<QuizGame> QuizGames { get; set; }
        public IDbSet<GamePlayer> GamePlayers { get; set; }
        public IDbSet<GameLevel> GameLevels { get; set; }
        public IDbSet<GameLog> GameLogs { get; set; }
        #endregion

        #region Training
        public IDbSet<TrainingPlan> TrainingPlans { get; set; }
        public IDbSet<TrainingGroup> TrainingGroups { get; set; }
        public IDbSet<TrainingGroupSubscription> TrainingGroupSubscriptions { get; set; }
        public IDbSet<TrainingSession> TrainingSessions { get; set; }
        public IDbSet<TrainingExercise> TrainingExercises { get; set; }

        public IDbSet<TrainingSchedule> TrainingSchedules { get; set; }

        public IDbSet<TrainingPractice> TrainingPractices { get; set; }

        public IDbSet<TrainingBenchmark> TrainingBenchmarks { get; set; }

        public IDbSet<TrainingBenchmarkRange> TrainingBenchmarkRanges { get; set; }
        #endregion

    }
}