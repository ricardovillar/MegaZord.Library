using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MegaZord.Library.MVC;
using System.Collections.Generic;

namespace MegaZord.Library.MVC
{

    public class MZRouteConfig
    {

        public static void RegisterRoutes(RouteCollection routes, IList<Route> customRoutes, IList<string> customViewSearchPath)
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new MZRazorViewEngine(customViewSearchPath));
            AreaRegistration.RegisterAllAreas();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");

            routes.MapRoute(
               "sitemap",
               "sitemap.xml",
               new { controller = "sitemap", action = "Index" }
           );

            routes.MapRoute(
                "Robots.txt",
                "Robots.txt",
                new { controller = "Robot", action = "Index" }
            );

            if ((customRoutes != null) && (customRoutes.Count > 0))
            {
                foreach (var r in customRoutes)
                {
                    routes.Add(r);
                }
            }
            routes.MapRoute(
                "Default", // Route name
                    "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
    
            ).RouteHandler = new MZMVCHyphenatedRouteHandler();


            // Add this code to handle non-existing urls
            routes.MapRoute(
                name: "404-PageNotFound",
                // This will handle any non-existing urls
                url: "{*url}",
                // "Shared" is the name of your error controller, and "Error" is the action/page
                // that handles all your custom errors
                defaults: new { controller = "Error", action = "Index" }
            ).RouteHandler = new MZMVCHyphenatedRouteHandler();
        }
    }
}