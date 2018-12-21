namespace EventSourcedCounter.Events
{
    public class SimpleEvent : EventBase
    {
        public SimpleEvent(string type)
            : base(type)
        {
        }
    }
}