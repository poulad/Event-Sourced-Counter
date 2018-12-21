using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EventSourcedCounter.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
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