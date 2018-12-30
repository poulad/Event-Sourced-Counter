using System.Threading;
using System.Threading.Tasks;
using ESC.Events;
using EventStore.ClientAPI;

namespace ESC.EventHandlers.Abstractions
{
    public interface IEventHandler<in TEvent>
        where TEvent : IEvent
    {
        Task HandleAsync(
            ResolvedEvent resolvedEvent,
            TEvent e,
            CancellationToken cancellationToken = default
        );
    }
}
