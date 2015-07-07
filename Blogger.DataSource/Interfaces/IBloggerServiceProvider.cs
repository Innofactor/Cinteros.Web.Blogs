using System.Collections.Generic;
using Google.Apis.Blogger.v3;

namespace Blogger.DataSource.Interfaces
{
    public interface IBloggerServiceProvider
    {
        BloggerService InitializeService();
        IEnumerable<string> GetUserBlogKeys();

    }
}