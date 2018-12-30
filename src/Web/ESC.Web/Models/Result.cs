using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESC.Web.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct Result
    {
        public bool Ok { get; set; }

        public string Message { get; set; }

        public object Value { get; set; }

        public string CorrelationId { get; set; }

        public Result(
            bool ok,
            string message = default,
            object value = default,
            string correlationId = default
        )
        {
            Ok = ok;
            Message = message;
            Value = value;
            CorrelationId = correlationId;
        }
    }
}