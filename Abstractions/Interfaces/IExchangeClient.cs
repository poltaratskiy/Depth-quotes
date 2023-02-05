using Abstractions.ProducerConsumerDto;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abstractions.Interfaces
{
    /// <summary> Exchange client that obtains depth quotes in real time and raises an event. </summary>
    public interface IExchangeClient
    {
        /// <summary> Connect to exchange and start to raise events. </summary>
        /// <param name="quoteReceived"> Delegate invoked when quote is received. </param>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> Task. </returns>
        public Task ConnectAsync(Func<Quote, Task> quoteReceived, CancellationToken cancellationToken = default);

        /// <summary> Close connection and stop raising events. </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> Task. </returns>
        public Task CloseConnectionAsync(CancellationToken cancellationToken = default);
    }
}
