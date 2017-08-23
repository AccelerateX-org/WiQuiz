using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;

namespace WIQuest.Web.Data
{
    [DataContract]
    public class QuestionAnswer
    {
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Antworttext
        /// </summary>
        [DataMember]
        [AllowHtml]

        public string Text { get; set; }

        /// <summary>
        /// Ein Bild (optional)
        /// </summary>
        [DataMember]
        public virtual BinaryStorage Image { get; set; }

        /// <summary>
        /// Antwort korrekt
        /// </summary>
        [DataMember]
        public bool IsCorrect { get; set; }

        public int Reihenfolge { get; set; }

        /// <summary>
        /// Erläuterung
        /// </summary>
        [AllowHtml]
        public string Explanation { get; set; }

        /// <summary>
        /// Zugehörige Frage
        /// </summary>
        public virtual Question Question { get; set; }

    }
}