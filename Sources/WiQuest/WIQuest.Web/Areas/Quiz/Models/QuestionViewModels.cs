using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Quiz.Models
{
    public class QuestionViewModel
    {
        public Question Question { get; set; }

        public List<QuestionAnswer> Answers { get; set; }

        public int Index { get; set; }
        public int Total { get; set; }
    }
}