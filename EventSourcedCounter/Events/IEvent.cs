using System;

namespace EventSourcedCounter.Events
{
    public interface IEvent
    {
        string Type { get; }

        DateTime? Timestamp { get; }
    }
}