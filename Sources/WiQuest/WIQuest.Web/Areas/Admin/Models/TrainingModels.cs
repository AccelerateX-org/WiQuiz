using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Areas.Admin.Models
{
    public class TrainingExerciseEditModel
    {
        public Guid ExerciseId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid QuizId { get; set; }

        public bool IsAvailable { get; set; }

        public string FromDate { get; set; }
        
        public string FromTime { get; set; }

        public string UntilDate { get; set; }

        public string UntilTime { get; set; }
    }
}