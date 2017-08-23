using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WIQuest.Web.Data
{
    public class GameLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// wann
        /// </summary>
        public DateTime LogDateTime { get; set; }

        /// <summary>
        /// zu welchem Spiel
        /// </summary>
        public virtual QuizGame Game { get; set; }

        /// <summary>
        /// hat welcher Spieler
        /// </summary>
        public virtual GamePlayer Player { get; set; }

        /// <summary>
        /// was geantwortet
        /// </summary>
        public virtual QuestionAnswer Answer { get; set; }
    }
}