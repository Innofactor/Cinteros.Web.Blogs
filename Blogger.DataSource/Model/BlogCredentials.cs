using Newtonsoft.Json;

namespace Blogger.DataSource
{
    internal class BlogCredentials
    {
        [JsonProperty(PropertyName = "blogId")]
        public string BlogId { get; set; }

        [JsonProperty(PropertyName = "blogkey")]
        public string BlogName { get; set; }
    }
}