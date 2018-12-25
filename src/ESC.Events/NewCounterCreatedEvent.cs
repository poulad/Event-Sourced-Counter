using ESC.Data.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESC.Events
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class NewCounterCreatedEvent : IEvent, ICorrelatedEvent, IUniqueIdentifierEvent
    {
        [JsonIgnore]
        public string Type => Types.NewCounterCreated;

        public Counter Counter { get; set; }

        public string CorrelationId { get; set; }

        public string CounterName => Counter?.Name;
    }
}
