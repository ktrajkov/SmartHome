using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SmartHome.Web.Startup))]
namespace SmartHome.Web
{
    public partial class Startup 
    {
        public void Configuration(IAppBuilder app) 
        {
            ConfigureAuth(app);
        }
    }
}
