using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESC.Events
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CounterIncrementedEvent : IEvent, ICorrelatedEvent, IUniqueIdentifierEvent
    {
        [JsonIgnore]
        public string Type => Types.CounterIncremented;

        public string CounterName { get; set; }

        public int By { get; set; }

        public long NewVersion { get; set; } // ToDo should record the version in the event

        public string CorrelationId { get; set; }
    }
}
