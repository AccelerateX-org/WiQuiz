using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class TrainingSession
    {
        public TrainingSession()
        {
            Exercises = new HashSet<TrainingExercise>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public TrainingBenchmark Benchmark { get; set; }


        public virtual TrainingPlan TrainingPlan { get; set; }

        public virtual ICollection<TrainingExercise> Exercises { get; set; }

    }
}