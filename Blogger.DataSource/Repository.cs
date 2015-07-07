using System;
using System.Collections.Generic;
using System.Linq;
using Blogger.DataSource.Interfaces;
using Blogger.DataSource.Model;
using Google.Apis.Blogger.v3;
using Google.Apis.Blogger.v3.Data;

namespace Blogger.DataSource
{
    public class Repository : IRepository, IDisposable
    {
        private readonly BloggerService _bloggerService;
        private readonly IBloggerServiceProvider _bloggerServiceProvider;

        public Repository(IBloggerServiceProvider bloggerServiceProvider)
        {
            _bloggerServiceProvider = bloggerServiceProvider;
            _bloggerService = _bloggerServiceProvider.InitializeService();
        }
        
        public IEnumerable<BlogInfo> GetBlogInfo()
        {
            var blogKeys = _bloggerServiceProvider.GetUserBlogKeys();

            var blogList = (from blogKey in blogKeys
                            let blogInfo = _bloggerService.Blogs.Get(blogKey).Execute()
                            select new BlogInfo
                            {
                                Title = blogInfo.Name,
                                Subtitle = blogInfo.Description,
                                Url = blogInfo.Url,
                                Updated = blogInfo.Updated,
                                BlogKey = blogKey
                            }).ToList();

            return blogList;
        }

        public IEnumerable<BlogPost> GetBlogPosts()
        {
            var blogList = new List<BlogPost>();
            var blogKeys = _bloggerServiceProvider.GetUserBlogKeys();


            foreach (var postList in blogKeys.Select(blogKey =>
                _bloggerService.Posts.List(blogKey).Execute()).Where(postList =>
                    postList.Items != null && postList.Items.Count != 0))
            {
                blogList.AddRange(ConvertToBlogPosts(postList));
            }
            return blogList;
        }

        public IEnumerable<BlogPost> SearchBlogPosts(string query)
        {
            var blogList = new List<BlogPost>();
            var blogKeys = _bloggerServiceProvider.GetUserBlogKeys();


            foreach (var postList in blogKeys.Select(blogKey =>
                _bloggerService.Posts.Search(blogKey, query).Execute()).Where(postList =>
                    postList.Items != null && postList.Items.Count != 0))
            {
                blogList.AddRange(ConvertToBlogPosts(postList));
            }
            return blogList;
        }

        private static IEnumerable<BlogPost> ConvertToBlogPosts(PostList postList)
        {
            return postList.Items.Select(post => new BlogPost(post.Blog.Id)
            {
                Author = new BlogAuthor
                {
                    ImageUrl = post.Author.Image.Url,
                    Name = post.Author.DisplayName
                },
                Content = post.Content,
                DataSourceUrl = post.Url,
                Tags = post.Labels,
                Published = post.Published ?? DateTime.MinValue,
                Title = post.Title
            }).ToList();
        }

        public void Dispose()
        {
            if (_bloggerService != null)
            {
                _bloggerService.Dispose();
            }
        }
    }
}
