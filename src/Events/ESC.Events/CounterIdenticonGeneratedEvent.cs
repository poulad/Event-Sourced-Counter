using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESC.Events
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CounterIdenticonGeneratedEvent : IEvent
    {
        [JsonIgnore]
        public string Type => Types.CounterIdenticonGenerated;

        public string Picture { get; set; }
    }
}
