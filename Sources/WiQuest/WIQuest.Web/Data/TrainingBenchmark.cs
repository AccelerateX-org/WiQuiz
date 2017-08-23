using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class TrainingBenchmark
    {
        public TrainingBenchmark()
        {
            Ranges = new HashSet<TrainingBenchmarkRange>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // Hat Levels
        // Jeder Level hat einen Bereich
        // einen Feedbacktext
        public virtual ICollection<TrainingBenchmarkRange> Ranges { get; set; }
    }
}