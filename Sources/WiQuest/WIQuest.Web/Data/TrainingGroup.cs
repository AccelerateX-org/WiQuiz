using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class TrainingGroup
    {
        public TrainingGroup()
        {
            Subscriptions = new HashSet<TrainingGroupSubscription>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public string AccessToken { get; set; }

        public virtual TrainingPlan TrainingPlan { get; set; }

        public virtual ICollection<TrainingGroupSubscription> Subscriptions { get; set; }

    }
}