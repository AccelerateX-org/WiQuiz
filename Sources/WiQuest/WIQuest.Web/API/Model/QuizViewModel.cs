using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using WIQuest.Web.Data;

namespace WIQuest.Web.API.Model
{
    [DataContract]
    public class QuizViewModel
    {
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Name des Quiz
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Beschreibungstext
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}