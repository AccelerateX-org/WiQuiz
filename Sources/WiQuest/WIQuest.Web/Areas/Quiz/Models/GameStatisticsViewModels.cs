using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIQuest.Web.Data;
using WIQuest.Web.Services;

namespace WIQuest.Web.Areas.Quiz.Models
{
    public class QuizGameStatisticsViewModel
    {
        public QuizGameStatisticsViewModel()
        {
            Games = new List<GameStatistics>();
        }

        public Data.Quiz Quiz { get; set; }

        public ICollection<GameStatistics> Games { get; private set; }
    }
}