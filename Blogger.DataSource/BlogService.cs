using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Blogger.DataSource.Interfaces;
using Blogger.DataSource.Model;
using Newtonsoft.Json;

namespace Blogger.DataSource
{
    public class BlogService
    {
        private readonly IRepository _repository;

        public BlogService(IRepository repository)
        {
            _repository = repository;
        }

        public BlogPostCollection GetPosts(int pageIndex = 0)
        {
            var blogPosts = _repository.GetBlogPosts();
            var postCollection = new BlogPostCollection(blogPosts.OrderByDescending(x => x.Published), pageIndex, 10);

            return postCollection;

        }

        public BlogPostCollection SearchPosts(string query, int pageIndex = 0)
        {

            var blogPosts = !string.IsNullOrWhiteSpace(query) ?
                _repository.SearchBlogPosts(query).ToList() : _repository
                .GetBlogPosts()
                .OrderByDescending(x => x.Published)
                .ToList();

            var postCollection = new BlogPostCollection(blogPosts, pageIndex, 10);
            return postCollection;

        }

        public BlogPostCollection GetTagPosts(string tags, int pageIndex = 0)
        {
            var blogPosts = _repository.GetBlogPosts();

            var enumerable = blogPosts as IList<BlogPost> ?? blogPosts.ToList();
            var postCollection = new BlogPostCollection(enumerable.Where(x => x.Tags
                .Contains(tags, StringComparer.OrdinalIgnoreCase))
                .OrderByDescending(x => x.Published)
                .ToList(), pageIndex, 10);

            return postCollection;
        }

        public BlogPostCollection GetArchivePosts(DateTime date, int pageIndex)
        {

            var blogPosts = _repository.GetBlogPosts();
            var postCollection = new BlogPostCollection(blogPosts.Where(x => x.Published.Year == date.Year && x.Published.Month == date.Month), pageIndex, 10);
            return postCollection;

        }

        public Dictionary<string, int> GetTagsMeta()
        {

            var blogPosts = _repository.GetBlogPosts().ToList();

            var tags = new List<string>();

            foreach (var tag in blogPosts.SelectMany(blogPost => blogPost.Tags.Where(tag => !tags.Contains(tag))))
            {
                tags.Add(tag);
            }

            var tagsDictionary = tags.ToDictionary(tag => tag, tag => blogPosts.Count(x => x.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)));
            var orderedDictionary = tagsDictionary.OrderBy(x => x.Key);
            var castedDictionary = orderedDictionary.ToDictionary(keyItem => keyItem.Key, valueItem => valueItem.Value);

            return castedDictionary;


        }

        public Dictionary<DateTime, int> GetArchiveCount()
        {
            var blogPosts = _repository.GetBlogPosts();
            var archiveCountDictionary = new Dictionary<DateTime, int>();

            foreach (var post in blogPosts)
            {
                if (archiveCountDictionary.Count(x => x.Key.Year == post.Published.Year
                    && x.Key.Month == post.Published.Month) != 0)
                {

                    archiveCountDictionary[archiveCountDictionary.FirstOrDefault(
                        x => x.Key.Year == post.Published.Year && x.Key.Month == post.Published.Month).Key] += 1;
                }
                else
                {
                    archiveCountDictionary.Add(post.Published, 1);
                }
            }
            var orderedDictionary = archiveCountDictionary.OrderByDescending(x => x.Key);
            var castedDictionary = orderedDictionary.ToDictionary(keyItem => keyItem.Key, valueItem => valueItem.Value);
            return castedDictionary;
        }

        public IEnumerable<BlogInfo> GetInfo()
        {
            return _repository.GetBlogInfo();
        }

    }
}
