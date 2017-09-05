using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WIQuest.Web.Startup))]
namespace WIQuest.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();

            // Die Datenbank muss existieren!
            GlobalConfiguration.Configuration.UseSqlServerStorage("LogDbContext");



            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthFilter() }
            });


            app.UseHangfireServer();
        }
    }
}
