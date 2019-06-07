using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "display", // Route name
                "display/{ip}/{port}", // URL with parameters
                new { controller = "Default", action = "display" }, // Parameter defaults
                new { ip = @"(\d*[\.]\d*)*" } // regular expression for the ip.
            );

            routes.MapRoute(
                "getArgsAndDisplay", // Route name
                "display/{ip}/{port}/{tempo}", // URL with parameters
                new { controller = "Default", action = "getArgsAndDisplay" }, // Parameter defaults
                new { ip = @"(\d*[\.]\d*)*" } // regular expression for the ip.
            );

            routes.MapRoute(
                "loadAndDisplay", // Route name
                "display/{path}/{tempo}", // URL with parameters
                new { controller = "Default", action = "loadAndDisplay" } // Parameter defaults
            );


            routes.MapRoute(
                "save",
                "save/{ip}/{port}/{tempo}/{duration}/{fileName}",
                new { controller = "Default", action = "save" }, // Parameter defaults
                new { ip = @"(\d*[\.]\d*)*" } // regular expression for the ip.
                );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}

