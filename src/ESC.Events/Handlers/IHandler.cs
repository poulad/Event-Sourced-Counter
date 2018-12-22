using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace ESC.Events.Handlers
{
    public interface IHandler
    {
        Task HandleAsync(RecordedEvent recordedEvent);
    }
}