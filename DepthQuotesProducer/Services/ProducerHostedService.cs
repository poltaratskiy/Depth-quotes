using Abstractions.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace DepthQuotesProducer.Services
{
    public class ProducerHostedService : IHostedService
    {
        private readonly IProducerService _producerService;

        public ProducerHostedService(IProducerService producerService)
        {
            _producerService = producerService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _producerService.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _producerService.StopAsync(cancellationToken);
        }
    }
}
