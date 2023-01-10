using Abstractions.CommonObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abstractions.Interfaces
{
    /// <summary> Exchange client that obtains depth quotes in real time and raises an event. </summary>
    public interface IExchangeClient
    {
        /// <summary> Connect to exchange and start to raise events. </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> Task. </returns>
        public Task ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary> Event related to obtaining quotes. </summary>
        public event EventHandler<QuoteReceivedEventArgs>? QuoteReceived;

        /// <summary> Close connection and stop raising events. </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> Task. </returns>
        public Task CloseConnectionAsync(CancellationToken cancellationToken = default);
    }
}
