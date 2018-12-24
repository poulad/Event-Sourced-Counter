using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESC.Events
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class NewCounterCreatedEvent : IEvent, ICorrelatedEvent
    {
        public string Name { get; set; }

        public string CorrelationId { get; set; }

        [JsonIgnore] public string Type => Types.NewCounterCreated;
    }
}