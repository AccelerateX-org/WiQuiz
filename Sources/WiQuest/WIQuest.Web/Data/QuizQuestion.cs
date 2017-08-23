using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WIQuest.Web.Data
{
    [DataContract]
    public class QuizQuestion
    {
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual QuizSection Section { get; set; }

        [DataMember]
        public virtual Question Question { get; set; }

        /// <summary>
        /// Reihenfolge der Frage in der Sektion
        /// </summary>
        [DataMember]
        public int Order { get; set; }
    }
}