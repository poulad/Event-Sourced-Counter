using System.Threading.Tasks;
using ESC.Events;

namespace ESC.EventHandlers.Services
{
    public interface IEventStoreClient
    {
        Task AppendEventAsync(
            string streamName,
            IEvent e,
            bool serializeEvent = true
        );
    }
}
