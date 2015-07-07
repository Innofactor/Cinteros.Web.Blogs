using Newtonsoft.Json;

namespace Blogger.DataSource.Model
{
    public class ServiceCredentials
    {
        [JsonProperty(PropertyName = "apiKey")]
        public string ApiKey { get; set; }
        [JsonProperty(PropertyName = "apiName")]
        public string ApiName { get; set; }
    }
}
