using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PdfSharp;
using PdfSharp.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using WIQuest.Web.Areas.Portfolio.Models;
using WIQuest.Web.Data;
using WIQuest.Web.Utils;

namespace WIQuest.Web.Areas.Portfolio.Controllers
{
    [AllowAnonymous]
    public class TestController : Controller
    {
        // GET: Portfolio/Test
        public ActionResult Index()
        {
            var x = @"* {
            margin: 0;
            padding: 0;
        }

        body {
            font: 12px/1.45 Helvetica,Arial,Sans-Serif;
        }

        main {
            display: block;
            margin: 0 auto;
            width: 100%;
        }

        h1 {
            font-size: 100%;
            margin-top: 1.5em;
        }

            h1 img {
                position: relative;
                top: 8px;
                left: -4px;
            }

        .ausdrucken {
            margin: 1em 0;
            border: 2px dotted #727272;
            padding: 1em;
            border-radius: 8px;
            position: relative;
        }

        .tip {
            font-style: italic;
            color: #464646;
            margin-bottom: 1em;
        }

        .mappe {
            border: 2px solid #727272;
            border-radius: 3px;
            width: 4em;
            height: 3em;
            position: absolute;
            right: 15px;
            top: 10px;
        }

            .mappe strong {
                position: absolute;
                left: 10px;
                top: 3px;
            }

        .aufkleber {
            background-color: #000;
            width: 30%;
            height: 1em;
            position: absolute;
            right: 4px;
            bottom: 4px;
        }

        h3 {
            font-size: 100%;
            font-weight: normal;
        }

        h2 {
            font-weight: normal;
            margin-bottom: 1em;
        }

        .familienname {
            font-weight: bold;
        }

        .anmeldungsnummer {
            border: 3px solid #727272;
            text-align: center;
            width: 4em;
            font-size: 100%;
            line-height: 1;
            color: #727272;
            padding: .6em 0;
            border-radius: 8px;
            position: absolute;
            right: 15px;
            top: 120px;
        }

        .laufendeNummer {
            color: #000;
            font-weight: bold;
        }

        .bestaetigung {
            border-top: 3px double #727272;
            margin-top: 1em;
            padding-top: 1em;
        }

        .unterschrift-zeile {
            margin: 3em 0;
        }

            .unterschrift-zeile span {
                border-top: 1px solid #727272;
                padding-right: 12em;
                padding-top: .3em;
            }

        .ort_datum {
            margin-right: 7.5em;
        }

        .mappeZurueck .ort_datum {
            margin-right: 5em;
        }

        .mappeZurueck .unterschrift-zeile {
            margin: 3em 0 0 0;
        }
";


            var model = new TestViewModel
            {
                Styles = x
            };

            return View(model);
        }

        public FileResult Email(TestViewModel data)
        {
            var model = new RegistrationViewModel
            {
                FirstName = "Olav",
                LastName =  "Hinz",
                EMail = "olav.hinz@hm.edu",
                Street = "Auenstr.",
                Number = "35",
                City = "München",
                PostalCode = "80469",
                Country = "Deutschland",
                Studienrichtung = "Fotodesign",
            };

            // pdf erzeugen
            // html rendern
            // streamen und dann als Attachment anhängen
            var stream = new MemoryStream();



            // E-Mail senden
            var email = new ApplicationEmail("Registration");
            email.To = model.EMail;
            email.From = "design@hm.edu";
            email.Subject = string.Format("Ihre Bewerbung für {0}", model.Studienrichtung);
            email.Application = model;
            email.Id = "123";
            email.Year = "2017/18";

            ViewBag.Styles = data.Styles;

            var html = this.RenderViewToString("_PrintOut", email);
            PdfDocument pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
            //pdf.Save("document.pdf");
            pdf.Save(stream, false);


            // Stream zurücksetzen
            stream.Position = 0;

            return File(stream, "text/pdf", "Ausdruck.pdf");

        }
    }
}