using System;
using System.Web;
using System.Web.Mvc;

using Cinteros.Web.Blogs.Website.Models;

namespace Cinteros.Web.Blogs.Website.Controllers
{
    public class BlogController : BaseController
    {
        public ActionResult Archive(int year, int month, int? page = 1)
        {
            var pageIndex = page.GetValueOrDefault(1) - 1; 

            var selection = BlogService.GetArchivePosts(new DateTime(year, month, 1), pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            ViewBag.Title = string.Format("Inlägg från {0}-{1}", year, month);
            return View("List", model);
        }

        public ActionResult Index(int? page = 1)
        {
            var pageIndex = page.GetValueOrDefault(1) - 1; 

            var selection = BlogService.GetPosts(pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1) };

            ViewBag.Title = "Senaste inläggen";
            return View("List", model);
        }

        public ActionResult Search(string q, int? page = 1)
        {
            var pageIndex = page.GetValueOrDefault(1) - 1; 

            var selection = BlogService.SearchPosts(q, pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            ViewBag.Title = string.Format("Sökresultat för '{0}'", q);
            return View("List", model);
        }

        public ActionResult Tag(string t, int? page = 1)
        {
            t = HttpUtility.UrlDecode(t);
            var pageIndex = page.GetValueOrDefault(1) - 1; 

            var selection = BlogService.GetTagPosts(t, pageIndex);

            var model = new BlogListViewModel { Selection = selection, PageIndex = page.GetValueOrDefault(1), };

            ViewBag.Title = string.Format("Inlägg taggade som '{0}'", t);
            return View("List", model);
        }
    }
}