using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESC.Events
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CounterValueIncrementedEvent : IEvent
    {
        public int Count { get; set; }

        [JsonIgnore] public string Type => Types.CounterValueIncremented;
    }
}