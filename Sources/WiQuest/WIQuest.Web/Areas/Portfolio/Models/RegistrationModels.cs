using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Areas.Portfolio.Models
{


    public class RegistrationViewModel
    {

        [Display(Name = "Vorname")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Familienname")]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "Mail-Adresse")]
        [Required]
        [EmailAddress]
        public string EMail { get; set; }

        [Display(Name = "Mail-Adresse (Wiederholung)")]
        [Required]
        [EmailAddress]
        public string EMailConfirmed { get; set; }

        [Display(Name = "Telefon-Nummer")]
        [Required]
        public string Phone { get; set; }

        [Display(Name = "Straße")]
        [Required]
        public string Street { get; set; }

        [Display(Name = "Hausnummer")]
        [Required]
        public string Number { get; set; }

        [Display(Name = "Stadt")]
        [Required]
        public string City { get; set; }

        [Display(Name = "PLZ")]
        [Required]
        public string PostalCode { get; set; }

        [Display(Name = "Land")]
        [Required]
        public string Country { get; set; }


        public string Studienrichtung { get; set; }

        public string Pruefung_schon_mal_gemacht { get; set; }

        public string Jahreszahl { get; set; }

        public string Studienrichtung_schon_mal_Aufnahmepruefung { get; set; }

        public string Pruefung_schon_mal_bestanden { get; set; }

        public string Bewerbungsmappe { get; set; }
    }


    public class ConfirmViewModel
    {
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

        public bool? ApplKD { get; set; }
        public bool? ApplID { get; set; }
        public bool? ApplFD { get; set; }
        public bool? ApplPassed { get; set; }

        public bool HasConfirmed { get; set; }
    }

}