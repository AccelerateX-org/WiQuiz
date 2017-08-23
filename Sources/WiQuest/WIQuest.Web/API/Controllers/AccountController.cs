using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WIQuest.Web.API.Responses;
using WIQuest.Web.Models;

namespace WIQuest.Web.API.Controllers
{
    public class AccountController : ApiController
    {

        protected ApplicationUserManager _userManager;


        protected AccountController()
        {
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? new ApplicationUserManager(new UserStore<ApplicationUser>(new UserDbContext()));
            }
            protected set
            {
                _userManager = value;
            }
        }


        public RegisterResponse GetRegister(string name, string email, string password)
        {
            var user = new ApplicationUser { UserName = name, Email = email };
            var result = UserManager.Create(user, password);

            var response = new RegisterResponse();
            response.error = !result.Succeeded;
            if (!result.Succeeded)
            {
                // fraglich
                //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                var sb = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    sb.AppendFormat("{0};", error);
                }

                response.error_msg = sb.ToString();
            }

            return response;
        }



        /// <summary>
        /// Abfrage der UserId
        /// </summary>
        /// <param name="UserName">Username oder Email-Adresse</param>
        /// <param name="Password">Das zum UserName dazugehörige Passwort</param>
        /// <returns>Die zum Account dazugehörige UserId</returns>
        public LoginResponse GetLogin(string name, string password)
        {
            // Hypothese: Login schlägt fehl - es kann keine UserId ermittelt werden
            var db = new UserDbContext();

            ApplicationUser user = null;
            var response = new LoginResponse();

            //Überprüfen ob Mail
            if (name.Contains("@"))
            {
                //Suche ob Mail vorhanden
                var tempUser = UserManager.FindByEmail(name);

                if (tempUser != null)
                {
                    //wenn was gefunden wurde, Überprüfen ob PW stimmt
                    user = UserManager.Find(tempUser.UserName, password);
                    //wenn pw vorhanden userID abfragen
                    if (user != null)
                    {
                        //UserID Abfragen
                        response.uid = user.Id;
                    }
                }
            }
            //Übergebener string ist evtl Loginname
            else
            {
                //Überprüfen ob vorhanden und PW stimmt
                user = UserManager.Find(name, password);

                //wenn user vorhanden stimmt userID abfragen
                if (user != null)
                {
                    //UserID Abfragen
                    response.uid = user.Id;
                }
            }

            return response;
        }


    }
}
