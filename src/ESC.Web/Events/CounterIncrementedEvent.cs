using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESC.Web.Events
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CounterIncrementedEvent : EventBase
    {
        public int Count { get; }

        public CounterIncrementedEvent(int count)
            : base(EventTypes.CounterIncremented)
        {
            Count = count;
        }
    }
}