using System;
using ESC.Data.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESC.Web.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CounterDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public long Count { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastModifiedAt { get; set; }

        public static explicit operator CounterDto(Counter entity) =>
            entity == null
                ? null
                : new CounterDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Count = entity.Count,
                    CreatedAt = entity.CreatedAt,
                    LastModifiedAt = entity.LastModifiedAt == default
                        ? null
                        : (DateTime?) entity.LastModifiedAt,
                };
    }
}
