using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Postal;

namespace WIQuest.Web.Areas.Portfolio.Models
{
    public class ApplicationEmail : Email
    {
        public ApplicationEmail(string registration) : base(registration)
        {
        }

        public string To { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public RegistrationViewModel Application { get; set; }

        public string Id { get; set; }

        public string Year { get; set; }

    }
}