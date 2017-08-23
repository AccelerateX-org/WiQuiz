using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WIQuest.Web.API.Responses
{
    [DataContract]
    public class GameList
    {
        [DataMember]

        public ICollection<GameSummary> Games { get; set; }
    }

    [DataContract]
    public class GameSummary
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Guid QuizId { get; set; }

        [DataMember]
        public int QuestionCount { get; set; }

        [DataMember]
        public int MyGameCount { get; set; }

        [DataMember]
        public int TotalGameCount { get; set; }

        [DataMember]
        public GameApproach BestGame { get; set; }

        [DataMember]
        public GameApproach LastGame { get; set; }
    }

    [DataContract]
    public class GameApproach
    {
        [DataMember]
        public DateTime PlayDateTime { get; set; }

        [DataMember]
        public int Score { get; set; }

        [DataMember]
        public int Rank { get; set; }
    }

    [DataContract]
    public class QuizGameCreate
    {
        [DataMember]
        public Guid GameId { get; set; }

        [DataMember]
        public Guid PlayerId { get; set; }

        [DataMember]
        public Data.Quiz Quiz { get; set; }

        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    public class QuizGameAnswerResult
    {
        [DataMember]
        public Guid QuizId { get; set; }

        [DataMember]
        public Guid AnswerId { get; set; }

        [DataMember]
        public bool IsCorrect { get; set; }

        [DataMember]
        public int Score { get; set; }

        [DataMember]
        public string Message { get; set; }

    }

    [DataContract]
    public class QuizGameResult
    {
        [DataMember]
        public Guid GameId { get; set; }

        [DataMember]
        public Guid PlayerId { get; set; }

        [DataMember]
        public Guid QuizId { get; set; }

        [DataMember]
        public int QuestionCount { get; set; }

        [DataMember]
        public int CorrectAnswerCount { get; set; }

        [DataMember]
        public int PersonalRank { get; set; }

        [DataMember]
        public int TotalRank { get; set; }


        [DataMember]
        public string Message { get; set; }
    }
}