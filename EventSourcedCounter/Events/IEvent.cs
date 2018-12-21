namespace EventSourcedCounter.Events
{
    public interface IEvent
    {
        string Type { get; }
    }
}