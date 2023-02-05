using Abstractions.ProducerConsumerDto;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Abstractions.Interfaces
{
    public interface IConsumer
    {
        /// <summary> Open connection to the server. </summary>
        /// <param name="quoteReceived"> Delegate invoked when quote is received. </param>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> Task. </returns>
        public Task ConnectAsync(Action<Quote> quoteReceived, CancellationToken cancellationToken = default);

        /// <summary> Close connection to server. </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> Task. </returns>
        public Task CloseConnectionAsync(CancellationToken cancellationToken = default);
    }
}
