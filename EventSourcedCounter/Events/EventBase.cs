using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EventSourcedCounter.Events
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public abstract class EventBase : IEvent
    {
        [JsonIgnore]
        public string Type { get; }

        [JsonIgnore]
        public DateTime? Timestamp { get; internal set; }
        
        protected EventBase(string type)
        {
            Type = type;
        }
    }
}