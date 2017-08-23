using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WIQuest.Web.Data
{
    public class GamePlayer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public Guid Id { get; set; }

        
        /// <summary>
        /// Fremdschlüssel auf Userdatenbank
        /// </summary>
        [DataMember]
        public string UserId { get; set; }

        /// <summary>
        /// Hat das Spiel initiiert
        /// </summary>
        [DataMember]
        public bool IsInitiator { get; set; }

        public virtual QuizGame Game { get; set; }
    }
}