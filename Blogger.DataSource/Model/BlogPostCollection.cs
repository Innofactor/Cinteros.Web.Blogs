using System;
using System.Collections.Generic;
using System.Linq;

namespace Blogger.DataSource.Model
{
    public class BlogPostCollection
    {
        /// <summary>
        /// Creates an instance of BlogPostCollection.
        /// </summary>
        /// <param name="postCollection"></param>
        /// <param name="pageIndex">The current page-index of pagination.</param>
        /// <param name="pageSize">Optional parameter for page-size. Defaults to value in configuration.</param>
        public BlogPostCollection(IEnumerable<BlogPost> postCollection, int pageIndex, int? pageSize = null)
        {
            
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "pageIndex", "The argument has to be a positive number of 0 or higher.");
            }

            pageSize = pageSize ?? 10;
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize", "The argument has to be a positive number above 0.");
            }

            SetFields(postCollection, pageIndex, pageSize.Value);
        }

        /// <summary>
        /// Gets if the selection has more items next in the pagination.
        /// </summary>
        public bool HasNextItems { get; private set; }

        /// <summary>
        /// Gets if the selection has more items previous in the pagination.
        /// </summary>
        public bool HasPreviousItems { get; private set; }

        /// <summary>
        /// Gets the current page-index of the pagination.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Gets the page-size of the pagination.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Gets the current posts in the pagination.
        /// </summary>
        public IEnumerable<BlogPost> Posts { get; set; }

        /// <summary>
        /// Gets the count of pages. Based on TotalPostCount and PageSize.
        /// </summary>
        public int PageCount { get; private set; }

        /// <summary>
        /// Gets the total count of posts, which are paginated over in the object.
        /// </summary>
        public int TotalPostCount { get; private set; }

        public BlogPostCollection ApplyTransformers()//BlogPostTransformersCollection transformers)
        {
            //if (transformers != null)
            //{
            //    transformers.ApplyTransformers(this.Posts);
            //}
            return this;
        }

        private void SetFields(IEnumerable<BlogPost> queryBlogPosts, int pageIndex, int pageSize)
        {
            
            PageIndex = pageIndex;
            PageSize = pageSize;

            TotalPostCount = queryBlogPosts.Count();
            PageCount = TotalPostCount / PageSize;

            var skip = GetSkip(PageIndex, PageSize);
            var take = PageSize;

            var pagedPosts = queryBlogPosts.Skip(skip).Take(take).ToList();
            Posts = pagedPosts;

            if (!queryBlogPosts.Any() || !pagedPosts.Any())
            {
                return;
            }

            HasNextItems = (queryBlogPosts.LastOrDefault().DataSourceUrl != pagedPosts.LastOrDefault().DataSourceUrl);
            HasPreviousItems = (queryBlogPosts.FirstOrDefault().DataSourceUrl != pagedPosts.FirstOrDefault().DataSourceUrl);
        }

        private static int GetSkip(int pageIndex, int pageSize)
        {
            return (pageIndex * pageSize);
        }
    }
}
