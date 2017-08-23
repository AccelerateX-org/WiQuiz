using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WIQuest.Web.Data
{
    [DataContract]
    public class Quiz
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Name des Quiz
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Beschreibungstext
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public virtual ICollection<QuizSection> Sections { get; set; }

        /// <summary>
        /// Zufällige Anordnung der Fragen
        /// </summary>
        public bool IsRandomByQuestion { get; set; }

        /// <summary>
        /// Zufällige Anordnung der Antworten
        /// </summary>
        public bool IsRandomByAnswer { get; set; }


        /// <summary>
        /// Anzeige der Erläuterungen nach Beantwortung
        /// </summary>
        public bool ShowExplanations { get; set; }

        /// <summary>
        /// obsolet => raus
        /// </summary>
        public bool IsPubslished { get; set; }

        /// <summary>
        /// obsolet => raus
        /// </summary>
        public int Level { get; set; }
    }
}