using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESC.Web.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class SetCountInputDto
    {
        // ToDo use data annotations to validate the input
        public int Count { get; set; }
    }
}
