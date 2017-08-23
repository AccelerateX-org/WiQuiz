using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WIQuest.Web.Data
{
    [DataContract]
    public class QuizSection
    {
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        /// <summary>
        /// Reihenfolge der Sektion innerhalb des Quiz
        /// </summary>
        [DataMember]
        public int Order { get; set; }

        public virtual Quiz Quiz { get; set; }

        [DataMember]
        public virtual ICollection<QuizQuestion> Questions { get; set; }

    }
}