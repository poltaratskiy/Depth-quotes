using Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace DepthQuotesProducer.Services
{
    public class ProducerService : IProducerService
    {
        private readonly ILogger _logger;
        private readonly IExchangeClient _exchangeClient;
        private readonly IProducer _producer;

        public ProducerService(ILogger<ProducerService> logger, IExchangeClient  exchangeClient, IProducer producer)
        {
            _logger = logger;
            _exchangeClient = exchangeClient;
            _producer = producer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _producer.ConnectAsync(cancellationToken);
            await _exchangeClient.ConnectAsync((quote) => _producer.SendQuoteAsync(quote, cancellationToken), cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _exchangeClient.CloseConnectionAsync(cancellationToken);
            await _producer.CloseConnectionAsync(cancellationToken);

            _logger.LogInformation("Connections closed");
        }
    }
}
