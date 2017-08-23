using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class TrainingPlan
    {
        public TrainingPlan()
        {
            Sessions = new HashSet<TrainingSession>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Beschreibung des Trainingsplans
        /// </summary>
        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public TrainingBenchmark Benchmark { get; set; }

        public virtual ICollection<TrainingSession> Sessions { get; set; }

        public virtual ICollection<TrainingGroup> Groups { get; set; }
    }
}