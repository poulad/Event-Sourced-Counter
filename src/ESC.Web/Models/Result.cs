using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESC.Web.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Result
    {
        public bool Ok { get; set; }

        public string Message { get; set; }

        public Result(bool ok, string message = default)
        {
            Ok = ok;
            Message = message;
        }
    }
}