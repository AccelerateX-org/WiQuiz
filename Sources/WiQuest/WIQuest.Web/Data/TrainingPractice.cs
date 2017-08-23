using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class TrainingPractice
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual TrainingExercise Exercise { get; set; }

        public virtual TrainingGroupSubscription Subscription { get; set; }

        public virtual QuizGame Game { get; set; }

        /* fraglich, da doppelt
         * und auch vom Ablauf her
        /// <summary>
        /// Erfolgsquote: Anteil richtig beantworteter Fragen
        /// </summary>
        public double SuccessRate { get; set; }
        */


    }
}