using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Admin.Models
{
    public class HighScoreViewModel
    {
        public HighScoreViewModel()
        {
        }

        public Data.Quiz Quiz { get; set; }

        public List<QuizGame> Games { get; set; }

        public List<GameLog> Logs { get; set; }

        public List<string> Players { get; set; }
    }
}