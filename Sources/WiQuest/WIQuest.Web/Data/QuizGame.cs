using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WIQuest.Web.Data
{
    public class QuizGame
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public DateTime CreatedAt { get; set; }

        [DataMember]
        public virtual ICollection<GameLevel> Levels { get; set; }

        [DataMember]
        public virtual ICollection<GamePlayer> Players { get; set; }
    }
}