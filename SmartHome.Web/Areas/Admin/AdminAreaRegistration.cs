using System.Web.Mvc;

namespace SmartHome.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "List", id = UrlParameter.Optional },
                 new[] { "SmartHome.Web.Areas.Admin.Controllers" }
            );
        }
    }
}