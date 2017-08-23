using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WIQuest.Web.Data;
using WIQuest.Web.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WIQuest.Web.Controllers
{
    public class BaseController : Controller
    {
        private QuestDbContext quizDb;
        private UserDbContext userDb;
        private UserManager<ApplicationUser> userManager;

        protected QuestDbContext QuizDb
        {
            get
            {
                if (quizDb == null)
                    quizDb = new QuestDbContext();
                return quizDb;
            }
        }

        protected UserDbContext UserDb
        {
            get
            {
                if (userDb == null)
                    userDb = new UserDbContext();
                return userDb;
            }
        }

        protected UserManager<ApplicationUser> UserManager
        {
            get
            {
                if (userManager == null)
                    userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(UserDb));
                return userManager;
            }
        }

        protected ApplicationUser GetCurrentUser()
        {
            return UserManager.FindByName(User.Identity.Name);
        }

        protected Quiz GetQuiz(Guid? id)
        {
            return QuizDb.Quizzes.Find(id);
        }

    }
}