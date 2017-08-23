using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WIQuest.Web.Models;

namespace WIQuest.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactMailModel model)
        {
            SmtpClient smtp = new SmtpClient();

            smtp.Send(model.Email, "hinz@hm.edu", "[wiquest] " + model.Subject, model.Body);

            return View("ThankYou");
        }


        public ActionResult Imprint()
        {
            return View();
        }
    }
}