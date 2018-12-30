namespace ESC.Events
{
    public interface ICorrelatedEvent
    {
        string CorrelationId { get; }
    }
}