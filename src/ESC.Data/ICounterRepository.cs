using System.Threading;
using System.Threading.Tasks;
using ESC.Data.Entities;

namespace ESC.Data
{
    public interface ICounterRepository
    {
        /// <summary>
        /// Adds a new counter entity
        /// </summary>
        /// <param name="counter">Counter entity</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        /// <returns>
        /// Returns <code>null</code> on success.
        /// Returns an error with the code <see cref="ErrorCodes.Duplicate"/>, if either
        /// the <see cref="Counter.Id"/> or the <see cref="Counter.Name"/> property is duplicate.
        /// </returns>
        Task<Error> AddAsync(
            Counter counter,
            CancellationToken cancellationToken = default
        );

        Task<(Counter Counter, Error Error)> GetByIdAsync(
            string id,
            CancellationToken cancellationToken = default
        );

        Task<(Counter Counter, Error Error)> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default
        );

        Task<Error> DeleteAsync(
            string id,
            CancellationToken cancellationToken = default
        );
    }
}
