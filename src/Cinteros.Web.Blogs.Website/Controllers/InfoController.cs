using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

using Cinteros.Web.Blogs.Website.Models;
using HtmlAgilityPack;

namespace Cinteros.Web.Blogs.Website.Controllers 
{
    public class InfoController : BaseController 
    {

        public ActionResult Archive() 
        {
            var postDates = BlogService.GetArchiveCount();

            return PartialView(postDates);
        }

        [OutputCache(Duration = DefaultCacheDuration)]
        public ActionResult Blogs()
        {
            var blogs = BlogService.GetInfo();

            return PartialView(blogs);
        }

        [OutputCache(Duration = DefaultCacheDuration)]
        public ActionResult Tags() {
            var tags = this.BlogService.GetTagsMeta();

            return PartialView(tags);
        }

        public static readonly string MenuItemsCacheKey = "InfoController.MenuItems.LastUpdate";

        [OutputCache(Duration = DefaultCacheDuration)]
        public ActionResult MenuItems()
        {
            //var storeMenuItems = new List<MenuItem>(0);

            //var lastUpdate = (this.HttpContext.Cache[MenuItemsCacheKey] as DateTime?).GetValueOrDefault(DateTime.MinValue);

            //var store = this.BlogService.DocumentStore;
            //using (var session = store.OpenSession())
            //{
            //    if (lastUpdate.AddHours(8) < DateTime.Now)
            //    {
            //        var storeHasData = session.Query<MenuItem>().Any();

            //        var task = new Task(() =>
            //        {
            //            var cinterosMenuItems = GetCinterosMenuItems();

            //            if (cinterosMenuItems.Any())
            //            {
            //                session.Query<MenuItem>().Take(int.MaxValue).ToList().ForEach(x => session.Delete(x));

            //                session.SaveChanges();
            //            }

            //            cinterosMenuItems.ToList().ForEach(x => session.Store(x, Convert.ToString((uint)x.Url.GetHashCode())));

            //            session.SaveChanges();
            //        });
            //        task.Start();

            //        if (!storeHasData)
            //        {
            //            task.Wait();
            //        }
            //    }

            //    storeMenuItems = session.Query<MenuItem>().Take(int.MaxValue).OrderBy(x => x.Index).ToList();
            //}

            HttpContext.Cache[MenuItemsCacheKey] = DateTime.Now;

            return PartialView(GetCinterosMenuItems());
        }

        private List<MenuItem> GetCinterosMenuItems() 
        {
            var client = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            string html = client.DownloadString("http://www.cinteros.se");

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var nodes = htmlDocument.DocumentNode.SelectNodes("//ul[@id='menu-cinteros-standard']/li");

            var menuItems = (from listItem in nodes
                let aTag = listItem.FirstChild
                where aTag.Name == "a"
                let href = aTag.GetAttributeValue("href", string.Empty)
                let linkUrl = GetUrl(href)
                where !string.IsNullOrWhiteSpace(href)
                select new MenuItem(aTag.InnerText, linkUrl)).ToList();
            for(int i = 0; i < menuItems.Count; i++) 
            {
                menuItems[i].Index = i;
            }

            // Modify the blog's link (if it's present) to the active menu-item
            var blogItem = menuItems.FirstOrDefault(item => item.Url.Contains("blogs.cinteros.se"));
            if(blogItem != null) {
                blogItem.Class += " current_page_item";
            }

            // Change class of last item to "last"
            var lastItem = menuItems.LastOrDefault();
            if(lastItem != null) {
                lastItem.Class += " last";
            }

            return menuItems;
        }

        private const string WebsiteUrl = "http://www.cinteros.se/";

        private static string GetUrl(string href) {
            if(href.StartsWith(WebsiteUrl)) {
                return href;
            }
            
            href = href.TrimStart('/');
            return (!href.StartsWith("http")) ? (WebsiteUrl + href) : href;
        }
    }
}