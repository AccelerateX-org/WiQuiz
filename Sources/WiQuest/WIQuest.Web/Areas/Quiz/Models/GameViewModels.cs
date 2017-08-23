using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIQuest.Web.Data;
using WIQuest.Web.Services;

namespace WIQuest.Web.Areas.Quiz.Models
{
    public class PersonalGameList
    {
        public PersonalGameList()
        {
            GameStats = new List<PersonalGameStat>();
            MySubscribedPlans = new List<TrainingPlan>();
            AvailablePlans = new List<TrainingPlan>();
            Exercises = new List<PersonalTrainingExercise>();
        }

        public List<PersonalGameStat> GameStats { get; set; }

        public List<TrainingPlan> MySubscribedPlans { get; set; }

        public List<TrainingPlan> AvailablePlans { get; set; }

        public List<PersonalTrainingExercise> Exercises { get; set; }
    }

    public class PersonalTrainingExercise
    {
        public TrainingGroupSubscription Subscription { get; set; }

        public TrainingExercise Exercise { get; set; }

    }

    public class PersonalGameStat
    {
        public Data.Quiz Quiz { get; set; }

        public List<Question> Questions { get; set; }

        public GameStatistics LastGame { get; set; }

        public GameStatistics BestGame { get; set; }

        public int PersonalRank { get; set; }

        public int TotalRank { get; set; }
    }


}