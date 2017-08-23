using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WIQuest.Web.Areas.SysAdmin.Models;
using WIQuest.Web.Models;

namespace WIQuest.Web.Areas.SysAdmin.Controllers
{
    public class UsersController : Controller
    {
        private UserDbContext db = new UserDbContext();

        // GET: SysAdmin/Users
        public ActionResult Index()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var model = new List<UserViewModel>();

            foreach (var user in db.Users.ToList())
            {
                var um = new UserViewModel
                {
                    User = user,
                    IsUserAdmin = UserManager.IsInRole(user.Id, "UserAdmin"),
                    IsQuizAdmin = UserManager.IsInRole(user.Id, "QuizAdmin"),
                };


                model.Add(um);
            }


            return View(model);
        }

        public ActionResult ToggleOff(string id, string role)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            if (RoleManager.RoleExists(role) && UserManager.IsInRole(id, role))
            {
                UserManager.RemoveFromRole(id, role);
            }
            
            return RedirectToAction("Index");
        }

        public ActionResult ToggleOn(string id, string role)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            if (!RoleManager.RoleExists(role))
            {
                RoleManager.Create(new IdentityRole(role));
            }

            // prüfen, ob schon drin
            if (!UserManager.IsInRole(id, role))
            {
                // das geht nur, wenn die Rolle existiert
                UserManager.AddToRole(id, role);
            }

            return RedirectToAction("Index");
        }

        public void dummy(string id)
        {
            /*
            var user = db.Users.SingleOrDefault(x => x.Id.Equals(id));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            RoleManager.Create(new IdentityRole("SysAdmin"));
            UserManager.IsInRole();
            */

        }
    }
}