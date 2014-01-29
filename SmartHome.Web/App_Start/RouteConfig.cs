using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SmartHome.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");           
            routes.MapRoute(
               null,
               "Houses/{houseName}",
                new { controller = "Houses", action = "Details"},
                new { httpMethod = new HttpMethodConstraint("GET") },
                 new[] { "SmartHome.Web.Controllers" }
            );

            routes.MapRoute(
                null,
                "Houses", 
                 new { controller = "Houses", action = "List"},
                  new[] { "SmartHome.Web.Controllers" }
             );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "SmartHome.Web.Controllers" }
                
            );
        }
    }
}
