using System;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Blogger.DataSource.Model;

namespace Cinteros.Web.Blogs.Website.Controllers {
    public class ServicesController : BaseController {
        private const string ContentType = "application/rss+xml";
        private const string CommunityTagName = "community";
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 30;

        public ActionResult BasicRss(int? pageSize = DefaultPageSize)
        {
            return GetDefaultRss(pageSize, includeBlogContent: false);
        }

        public ActionResult Rss(int? pageSize = DefaultPageSize)
        {
            return GetDefaultRss(pageSize, includeBlogContent: true);
        }

        public ActionResult CommunityRss(int? pageSize = DefaultPageSize)
        {
            int actualPageSize = Math.Min(pageSize.GetValueOrDefault(DefaultPageSize), MaxPageSize);
           // this.BlogService.Config.PageSize = actualPageSize;

            var selection = this.BlogService.GetTagPosts(CommunityTagName, 0);
            return GetRssFeed(selection, includeBlogContent: true);
        }

        private ActionResult GetDefaultRss(int? pageSize = DefaultPageSize, bool includeBlogContent = true)
        {
            int actualPageSize = Math.Min(pageSize.GetValueOrDefault(DefaultPageSize), MaxPageSize);
            //this.BlogService.Config.PageSize = actualPageSize;

            var selection = this.BlogService.GetPosts(0);
            return GetRssFeed(selection, includeBlogContent);
        }

        private ActionResult GetRssFeed(BlogPostCollection selection, bool includeBlogContent = true) {
            Response.ContentType = ContentType;

            var rssItems = from post in selection.Posts
                           select GetPostXElement(post, includeBlogContent);

            string rssUrl = "http://" + Request.Url.Host + Url.RouteUrl("Empty", null);

            var rss = new XDocument(
                          new XDeclaration("1.0", "UTF-8", "yes"),
                          new XElement("rss",
                              new XAttribute("version", "2.0"),
                              new XElement("channel",
                                  new XElement("title", "Cinteros Blogs"),
                                  new XElement("link", rssUrl),
                                  new XElement("description", "RSS feed containing blog-posts from bloggers at Cinteros AB."),
                                  rssItems
                              )
                          )
                      );

            return Content(rss.ToString(), ContentType);
        }

        private static XElement GetPostXElement(BlogPost post, bool includeBlogContent) {
            var itemElements = new[] {
                new XElement("title", post.Title),
                new XElement("link", post.DataSourceUrl),
                new XElement("pubDate", post.Published.ToString("r")),
                new XElement("author", post.Author.Name),
            };
            if(includeBlogContent) {
                itemElements = itemElements.Concat(new[] {
                    new XElement("description", post.Content),
                }).ToArray();
            }

            return new XElement("item",
                itemElements
            );
        }

        //public ActionResult RefreshBlogs()
        //{
        //    var refreshResults = this.BlogService.Refresh();

        //    return Json(refreshResults.Select(x =>
        //        new { Blog = x.BlogKey, Elapsed = x.ElapsedTime.TotalSeconds.ToString("0.###s") }),
        //        JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ClearMenuItems() {
            this.HttpContext.Cache.Remove(InfoController.MenuItemsCacheKey);

            return this.RedirectToRoute("Empty");
        }
    }
}