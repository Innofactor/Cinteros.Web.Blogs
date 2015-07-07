using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blogger.DataSource.Interfaces;
using Blogger.DataSource.Model;
using Google.Apis.Blogger.v3;
using Google.Apis.Services;
using Newtonsoft.Json;

namespace Blogger.DataSource
{
    public class BloggerServiceProvider : IBloggerServiceProvider
    {

        public BloggerService InitializeService()
        {
            var path = System.Configuration.ConfigurationManager.AppSettings["ServiceCredentialsUri"];
            var serviceCredentials = GetServiceCredentials(path);

            var bloggerService = new BloggerService(new BaseClientService.Initializer
            {
                ApplicationName = serviceCredentials.ApiName,
                ApiKey = serviceCredentials.ApiKey
            });

            return bloggerService;
        }

        public IEnumerable<string> GetUserBlogKeys()
        {
            try
            {
                var path = System.Configuration.ConfigurationManager.AppSettings["BlogCredentialsUri"];
                List<BlogCredentials> blogCredentials;

                using (var streamreader = new StreamReader(path))
                {
                    var json = streamreader.ReadToEnd();

                    blogCredentials = JsonConvert.DeserializeObject<List<BlogCredentials>>(json);
                }
                var credentials = blogCredentials.Select(x => x.BlogId).ToList();
                return credentials;
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception(
                    string.Format("File not found with error message: {0}", ex.Message));
            }

        }

        private static ServiceCredentials GetServiceCredentials(string path)
        {
            ServiceCredentials serviceCredentials;

            using (var streamReader = new StreamReader(path))
            {
                var json = streamReader.ReadToEnd();

                serviceCredentials = JsonConvert.DeserializeObject<ServiceCredentials>(json);
            }

            return serviceCredentials;

        }
    }
}