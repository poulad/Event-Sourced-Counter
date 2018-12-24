using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESC.Events
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CounterValueIncrementedEvent : IEvent, ICorrelatedEvent
    {
        [JsonIgnore]
        public string Type => Types.CounterValueIncremented;

        public int Count { get; set; }

        public string CorrelationId { get; set; }
    }
}
