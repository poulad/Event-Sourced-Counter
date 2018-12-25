using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESC.Events
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CounterIncrementRequestedEvent : IEvent, ICorrelatedEvent, IUniqueIdentifierEvent
    {
        [JsonIgnore]
        public string Type => Types.CounterIncrementRequested;

        public string CounterName { get; set; }

        public int By { get; set; }

        public string CorrelationId { get; set; }
    }
}
