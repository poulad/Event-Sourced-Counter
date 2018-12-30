using System.Threading.Tasks;

namespace ESC.Events.Handlers.Services
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
