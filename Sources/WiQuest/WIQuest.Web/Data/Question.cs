using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;

namespace WIQuest.Web.Data
{
    [DataContract]
    public class Question
    {
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        /// <summary>
        /// Falls eine Frage mal aus einem anderen System kommen sollte
        /// </summary>
        [DataMember]
        public string ExternalId { get; set; }

        /// <summary>
        /// Titel der Frage - optional
        /// </summary>
        [DataMember]
        [DisplayName("Fragetitel (optional)")]
        public string Title { get; set; }


        /// <summary>
        /// Fragetext
        /// </summary>
        [DataMember]
        [DisplayName("Fragetext")]
        [AllowHtml]
        public string Text { get; set; }

        /// <summary>
        /// Schwierigkeitsgrad
        /// 100 - Einstieger
        /// 200 - Fortgeschritten
        /// 300 - Profi
        /// </summary>
        [DataMember]
        [DisplayName("Schwierigkeitsgrad (100, 200, 300)")]
        public int Level { get; set; }

        /// <summary>
        /// Reihenfolge innerhalb der Kategorie
        /// </summary>
        public int Reihenfolge { get; set; }


        /// <summary>
        /// Ein Bild (Optional)
        /// </summary>
        [DataMember]
        [DisplayName("Image")]
        public virtual BinaryStorage Image { get; set; }


        /// <summary>
        /// Zugehörige Kategorie der Frage
        /// </summary>
        [DataMember]
        [DisplayName("Kategorie (veraltet)")]
        public virtual QuestionCategory Category { get; set; }

        /// <summary>
        /// Liste der Schlagworte, unter denen die Frage gefunden werden kann
        /// </summary>
        public virtual ICollection<QuestionTag> Tags { get; set; }


        /// <summary>
        /// Liste der zugehörigen Antworten
        /// </summary>
        [DataMember]
        public virtual ICollection<QuestionAnswer> Answers { get; set; }


        /// <summary>
        /// Liste der Quizzes, in der die Frage verwendet wird
        /// </summary>
        public virtual ICollection<QuizQuestion> QuizQuestions { get; set; }

    }
}