using System.Threading;
using System.Threading.Tasks;
using ESC.Events;

namespace ESC.Web.Services
{
    public interface IEventStoreClient
    {
        Task AppendMutationRequestEventAsync(
            IEvent e,
            bool serializeEvent = true,
            CancellationToken cancellationToken = default
        );
    }
}
