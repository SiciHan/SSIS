using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Web.ApplicationServices;
using Microsoft.Owin;
using Owin;
using Team8ADProjectSSIS.DAO;

[assembly: OwinStartup(typeof(Team8ADProjectSSIS.Startup))]

namespace Team8ADProjectSSIS
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.MapSignalR();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddScoped<ICategoryDAO, CategoryDAO>();
        }
    }
}
