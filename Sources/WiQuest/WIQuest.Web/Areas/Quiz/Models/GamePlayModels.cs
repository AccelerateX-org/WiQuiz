using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Quiz.Models
{
    public class GamePlayState
    {
        public GamePlayState()
        {
            QuestionsLeft = new List<Question>();
        }

        public Data.QuizGame Game { get; set; }

        public Data.GamePlayer Player { get; set; }

        public Data.Quiz Quiz { get; set; }

        public TrainingPractice Practice { get; set; }

        /// <summary>
        /// noch zu beantwortende Fragen
        /// </summary>
        public List<Question> QuestionsLeft { get; set; }

        public int QuestionTotalCount { get; set; }

        public int CurrentQuestionIndex { get; set; }

        public int CurrentNumberOfCorrectAnswers { get; set; }

        public QuestionViewModel CurrentQuestion { get; set; }

    }

    public class QuestionAnswerViewModel
    {
        public List<QuestionAnswer> Answers { get; set; }

        public QuestionAnswer GivenAnswer { get; set; }

        public QuestionAnswer CorrectAnswer { get; set; }
    }

}