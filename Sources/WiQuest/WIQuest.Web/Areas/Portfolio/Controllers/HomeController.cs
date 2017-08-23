using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using PdfSharp;
using PdfSharp.Pdf;
using WIQuest.Web.Areas.Portfolio.Models;
using WIQuest.Web.Utils;
using Postal;
//using Spire.Pdf;
//using Spire.Pdf.General.Render.Font;
using Spire.Pdf.HtmlConverter;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Portfolio.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        // GET: Portfolio/Home
        public ActionResult Index()
        {
#if DEBUG
            var model = new RegistrationViewModel
            {
                FirstName = "Olav",
                LastName = "Hinz",
                EMail = "hinz@hm.edu",
                EMailConfirmed = "hinz@hm.edu",
                Phone = "089",
                Street = "Au",
                Number = "35",
                City = "Muc",
                Country = "De",
                PostalCode = "80354",
                Studienrichtung = "Fotodesign",
                Bewerbungsmappe = "",
                Jahreszahl = "2003",
                Pruefung_schon_mal_bestanden = "nein",
                Pruefung_schon_mal_gemacht = "ja",
                Studienrichtung_schon_mal_Aufnahmepruefung = "Industriedesign"
            };

            return View("Edit", model);
#else
            return View("Edit");
#endif
        }

        public ActionResult Edit2()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Edit(RegistrationViewModel model)
        {
            if (!string.IsNullOrEmpty(model.EMail) &&
                !string.IsNullOrEmpty(model.EMailConfirmed) &&
                !model.EMail.Equals(model.EMailConfirmed))
            {
                ModelState.AddModelError("Email", "Die beiden Mail Adressen sind nicht identisch");
                ModelState.AddModelError("EMaiLConfirmed", "Die beiden Mail Adressen sind nicht identisch");
            }

            if (!ModelState.IsValid)
                return View();

            Session["Application"] = model;
            /*
            var confirmModel = new ConfirmViewModel();
            confirmModel.Curriculum = model.Curriculum;
            confirmModel.FirstName = model.FirstName;
            confirmModel.LastName = model.LastName;
            confirmModel.EMail = model.EMail;
            */

            return View("Confirm", model);

            // geht ist aber suboptimal, weil Parameter dann über get
            //return RedirectToAction("Confirm", model);
        }


        /*
        [HttpPost]
        public ActionResult Confirm(RegistrationViewModel model, string submitButton)
        {
            if (submitButton.Equals("abschicken"))
            {
                // In Datenbank speichern

                // E-Mail senden

                return View("ThankYou");
            }

            return View("Index", model);
        }
         */
        
        public ActionResult Save()
        {
            // In Datenbank speichern
            var model = (RegistrationViewModel)Session["Application"];

            var db = new PortfolioDbContext();

            var c = 0;
            if (model.Studienrichtung.Equals("Fotodesign")) c = 1;
            if (model.Studienrichtung.Equals("Industriedesign")) c = 2;
            if (model.Studienrichtung.Equals("Kommunikationsdesign")) c = 3;

            bool hist = model.Pruefung_schon_mal_gemacht.Equals("ja");

            var c2 = 0;
            bool b = false;
            if (hist)
            {
                if (model.Studienrichtung_schon_mal_Aufnahmepruefung.Equals("Fotodesign")) c2 = 1;
                if (model.Studienrichtung_schon_mal_Aufnahmepruefung.Equals("Industriedesign")) c2 = 2;
                if (model.Studienrichtung_schon_mal_Aufnahmepruefung.Equals("Kommunikationsdesign")) c2 = 3;

                b = model.Pruefung_schon_mal_bestanden.Equals("ja");
            }

            var appl = new PortfolioApplication
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Curriculum = c,
                EMail = model.EMail,
                Phone = model.Phone,
                Street = model.Street,
                Number = model.Number,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country,
                RegistrationDate = DateTime.Now,
                HasPrevAppl = hist,
                PrevApplSem = hist ? model.Jahreszahl : null,
                PrevCurriculum = hist ? (int?)c2 : null,
                ApplPassed = hist ? (bool?)b : (bool?) null,
                HasConfirmed = true
            };

            db.Applications.Add(appl);
            db.SaveChanges();
            
            
            // pdf erzeugen
            // html rendern
            /*
            var doc = new PdfDocument();
            // html als string übergeben

            var html = this.RenderViewToString("_PrintOut", model);
            var pageSettings = new PdfPageSettings
            {
                Height = 297F,
                Width = 210F,
                Orientation = PdfPageOrientation.Portrait,
            };
            var pdfLayoutFormat = new PdfHtmlLayoutFormat();


            Thread thread = new Thread(() =>
            {
                doc.LoadFromHTML(html, true, pageSettings, pdfLayoutFormat);
            });
	         thread.SetApartmentState(ApartmentState.STA);
	         thread.Start();
	         thread.Join();
            */


            // streamen und dann als Attachment anhängen
            var stream = new MemoryStream();
            //doc.SaveToStream(stream);
            /*
            var path = Path.Combine(Path.GetTempPath(), "Ausdruck.pdf");
            doc.SaveToFile(path);
            */



            // E-Mail senden
            var email = new ApplicationEmail("Registration");
            email.To = model.EMail;
            email.From = "design@hm.edu";
            email.Subject = string.Format("Ihre Bewerbung für {0}", model.Studienrichtung);
            email.Application = model;
            email.Id = appl.Id.ToString();
            email.Year = "2017/18";

            var html = this.RenderViewToString("_PrintOut", email);
            PdfDocument pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
            //pdf.Save("document.pdf");
            pdf.Save(stream, false);


            // Stream zurücksetzen
            stream.Position = 0;
            email.Attach(new Attachment(stream, "Aufkleber.pdf", System.Net.Mime.MediaTypeNames.Application.Pdf));
            //email.Attach(new Attachment(path, System.Net.Mime.MediaTypeNames.Application.Pdf));

            email.Send();


            return View("ThankYou");
        }

        public ActionResult Correct()
        {
            var model = (RegistrationViewModel)Session["Application"];

            return View("Edit", model);
        }
    }
}