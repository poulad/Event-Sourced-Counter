using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESC.Data.Redis.Entities
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Counter
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public long Value { get; set; }

        public long Version { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastModifiedAt { get; set; }
    }
}