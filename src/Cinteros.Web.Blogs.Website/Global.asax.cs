﻿using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Blaven;
using Raven.Client;
using Raven.Client.Document;
using System;

namespace Cinteros.Web.Blogs.Website {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            routes.MapRoute(
                "Archive",
                "{year}/{month}",
                new { controller = "Blog", action = "Archive", },
                new { year = @"\d+", month = @"\d+", }
            );

            routes.MapRoute(
                "Search",
                "search",
                new { controller = "Blog", action = "Search", }
            );

            routes.MapRoute(
                "Tag",
                "tag",
                new { controller = "Blog", action = "Tag", }
            );

            /*routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Blog", action = "Index", id = UrlParameter.Optional }
            );*/

            routes.MapRoute(
                name: "Info",
                url: "Info/{action}",
                defaults: new { controller = "Info", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Services",
                url: "Services/{action}",
                defaults: new { controller = "Services", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "RSS",
                url: "rss",
                defaults: new { controller = "Services", action = "Rss" }
            );

            routes.MapRoute(
                name: "CommunityRSS",
                url: "community-rss",
                defaults: new { controller = "Services", action = "CommunityRss" }
            );

            routes.MapRoute(
                name: "Empty",
                url: "",
                defaults: new { controller = "Blog", action = "Index" }
            );
        }

        private static DateTime _lastUnstaleIndexes;
        public override string GetVaryByCustomString(HttpContext context, string custom) {
            if(custom.Equals("RavenDbStaleIndexes", System.StringComparison.InvariantCultureIgnoreCase)) {
                bool staleIndexes = DocumentStore.DatabaseCommands.GetStatistics().StaleIndexes.Any();
                if(!staleIndexes) {
                    _lastUnstaleIndexes = DateTime.Now;
                    return string.Empty;
                }

                return string.Format("StaleIndexsFrom_{0}", _lastUnstaleIndexes.Ticks);
            }

            return base.GetVaryByCustomString(context, custom);
        }

        public static IDocumentStore DocumentStore { get; set; }

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            SetupBloggerViewController();
        }

        private static void SetupBloggerViewController() {
            DocumentStore = BlogService.GetDefaultBlogStore();
            
            // Init Blaven config
            StartWatchConfig(AppSettingsService.BloggerSettingsPath);
        }

        private static FileSystemWatcher _configWatcher;
        private static void StartWatchConfig(string filePath) {
            var fileInfo = new FileInfo(filePath);
            _configWatcher = new FileSystemWatcher(fileInfo.Directory.FullName, fileInfo.Name);

            _configWatcher.Changed += new FileSystemEventHandler(ConfigWatcher_Changed);
            _configWatcher.EnableRaisingEvents = true;
        }

        private static void ConfigWatcher_Changed(object sender, FileSystemEventArgs e) {
            HttpRuntime.UnloadAppDomain();
        }
    }
}