using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WIQuest.Web.Data
{
    public class TrainingExercise
    {
        public TrainingExercise()
        {
            Schedules = new HashSet<TrainingSchedule>();
            Practices = new HashSet<TrainingPractice>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public TrainingBenchmark Benchmark { get; set; }


        public virtual TrainingSession TrainingSession { get; set; }

        public virtual Quiz Quiz { get; set; }

        public bool IsAvailabe { get; set; }

        public DateTime? AvailableFrom { get; set; }

        public DateTime? AvailableUntil { get; set; }

        public virtual ICollection<TrainingSchedule> Schedules { get; set; }

        public virtual ICollection<TrainingPractice> Practices { get; set; }
    }
}