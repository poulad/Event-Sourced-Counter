using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace ESC.Events.Handlers
{
    public interface IEventHandler<in TEvent>
        where TEvent : IEvent
    {
        Task Handle(
            ResolvedEvent resolvedEvent,
            TEvent e
        );
    }
}