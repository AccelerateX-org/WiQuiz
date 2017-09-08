using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.Owin;

namespace WIQuest.Web
{
    public class HangFireAuthFilter : IDashboardAuthorizationFilter
    {
     public bool Authorize(IDictionary<string, object> owinEnvironment)
     {
         // In case you need an OWIN context, use the next line,
         // `OwinContext` class is the part of the `Microsoft.Owin` package.
         var context = new OwinContext(owinEnvironment);

         // Allow all authenticated users to see the Dashboard (potentially dangerous).
         return context.Authentication.User.Identity.IsAuthenticated;
     }

        public bool Authorize([NotNull] DashboardContext context)
        {
            throw new NotImplementedException();
        }
    }

}
