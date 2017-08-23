using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using WIQuest.Web.Data;
using WIQuest.Web.Hubs;

namespace WIQuest.Web.Services
{
    public class StatisticsService
    {
        private readonly QuestDbContext _dbContext;

        public StatisticsService(QuestDbContext db)
        {
            _dbContext = db;
        }

        public void NotifyUpdates()
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<StatisticsHub>();

            if (hubContext != null)
            {
                var statistics = new Statistics();

                statistics.TotalAnswers = _dbContext.GameLogs.Count();
                statistics.CorrectAnswers = _dbContext.GameLogs.Count(x => x.Answer.IsCorrect);

                hubContext.Clients.All.updateStatistics(statistics);
            }
        }

    }

    public class Statistics
    {
        public int TotalAnswers { get; set; }

        public int CorrectAnswers { get; set; }
    }
}