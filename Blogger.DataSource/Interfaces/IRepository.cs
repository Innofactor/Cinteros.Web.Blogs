using System.Collections.Generic;
using Blogger.DataSource.Model;

namespace Blogger.DataSource.Interfaces
{
    public interface IRepository
    {
        IEnumerable<BlogPost> GetBlogPosts();
        IEnumerable<BlogPost> SearchBlogPosts(string query);
        IEnumerable<BlogInfo> GetBlogInfo();
    }
}