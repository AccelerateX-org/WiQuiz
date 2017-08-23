using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Models
{
    public class QuestionModel
    {
        public QuestionModel()
        {
            Tags = new List<string>();
            Answers = new List<QuestionAnswerModel>();
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public int Level { get; set; }

        public string Topic { get; set; }

        public string ImageFileName { get; set; }

        public ICollection<string> Tags { get; set; }

        public ICollection<QuestionAnswerModel> Answers { get; set; }
    }

    public class QuestionAnswerModel
    {
        public string Text { get; set; }

        public bool IsCorrect { get; set; }

        public int Order { get; set; }

        public string HelpText { get; set; }

        public string ImageFileName { get; set; }

    }
}