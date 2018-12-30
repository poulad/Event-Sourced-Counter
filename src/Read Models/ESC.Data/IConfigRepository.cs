using System.Threading;
using System.Threading.Tasks;

namespace ESC.Data
{
    public interface IConfigRepository
    {
        Task SetLastProcessedEventIdAsync(
            string stream,
            long position,
            CancellationToken cancellationToken = default
        );

        Task<long?> GetLastProcessedEventIdAsync(
            string stream,
            CancellationToken cancellationToken = default
        );
    }
}
