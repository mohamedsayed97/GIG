using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ICP_ABC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapMvcAttributeRoutes();
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
           "Default2",
           "{area}/{controller}/{action}/{id}",
           new { controller = "Account", action = "Login", area = "Account", id = UrlParameter.Optional }
       );
            routes.MapRoute(
            "Default3",
            "{controller}/{action}/{id}",
            new {controller = "Account", action = "Login", id = UrlParameter.Optional }
        );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Account", action = "Login" , id=UrlParameter.Optional }
            //);
        }
    }
}
