using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EventSourcedCounter.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Counter
    {
        public string Name { get; set; }

        public long Value { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastModifiedAt { get; set; }
    }
}