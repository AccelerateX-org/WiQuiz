using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class PortfolioApplication
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Curriculum { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EMail { get; set; }

        public string Phone { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }


        public bool HasPrevAppl { get; set; }

        public string PrevApplSem { get; set; }

        public int? PrevCurriculum { get; set; }

        public bool? ApplPassed { get; set; }

        public bool HasConfirmed { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public DateTime? AcceptanceDate { get; set; }

        public bool? IsAccepted { get; set; }



    }
}