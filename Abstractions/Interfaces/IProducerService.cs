using System.Threading;
using System.Threading.Tasks;

namespace Abstractions.Interfaces
{
    /// <summary> Component obtaining quotes and producing to specified transport. </summary>
    public interface IProducerService
    {
        /// <summary> Start obtaining quotes from exchange and producing. </summary>
        public Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary> Stop sending quotes. </summary>
        public Task StopAsync(CancellationToken cancellationToken = default);
    }
}
