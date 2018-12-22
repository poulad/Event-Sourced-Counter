using Newtonsoft.Json;

namespace ESC.Web.Events
{
    public abstract class EventBase : IEvent
    {
        [JsonIgnore]
        public string Type { get; }
        
        protected EventBase(string type)
        {
            Type = type;
        }
    }
}