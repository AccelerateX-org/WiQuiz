using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class TrainingSchedule
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual TrainingGroup Group { get; set; }

        public virtual TrainingExercise Exercise { get; set; }

        public bool IsFromRestricted { get; set; }

        public bool IsUntilRestricted { get; set; }

        public DateTime? AvailableFrom { get; set; }

        public DateTime? AvailableUntil { get; set; }

    }
}