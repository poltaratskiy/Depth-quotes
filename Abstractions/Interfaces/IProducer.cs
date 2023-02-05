using Abstractions.ProducerConsumerDto;
using System.Threading;
using System.Threading.Tasks;

namespace Abstractions.Interfaces
{
    /// <summary> Producer sends quotes to streams. </summary>
    public interface IProducer
    {
        /// <summary> Open connection to the server. </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> Task. </returns>
        public Task ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary> Send quote obtained from exchange. </summary>
        /// <param name="quote"> Quote. </param>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> Task. </returns>
        public Task SendQuoteAsync(Quote quote, CancellationToken cancellationToken = default);

        /// <summary> Close connection to server. </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> Task. </returns>
        public Task CloseConnectionAsync(CancellationToken cancellationToken = default);
    }
}
