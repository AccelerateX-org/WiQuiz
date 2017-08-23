using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class TrainingBenchmarkRange
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual TrainingBenchmark Benchmark { get; set; }

        public double LowerBorder { get; set; }

        public double UpperBorder { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Feedback { get; set; }

    }
}