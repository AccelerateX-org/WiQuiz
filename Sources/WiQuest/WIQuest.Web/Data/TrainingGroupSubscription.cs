using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class TrainingGroupSubscription
    {
        public TrainingGroupSubscription()
        {
            Practices = new HashSet<TrainingPractice>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Userid { get; set; }

        public DateTime TimeStamp { get; set; }

        public virtual TrainingGroup TrainingGroup { get; set; }

        public virtual ICollection<TrainingPractice> Practices { get; set; }
    }
}