using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WIQuest.Web.Data
{
    [DataContract]
    public class QuestionCategory
    {
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [DataMember]
        public string ShortName { get; set; }

        [DataMember]
        public string Name { get; set; }

        public int Reihenfolge { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}