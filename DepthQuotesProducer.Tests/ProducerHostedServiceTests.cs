using Abstractions.Interfaces;
using DepthQuotesProducer.Services;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace DepthQuotesProducer.Tests
{
    internal class ProducerHostedServiceTests
    {
        [Test]
        public async Task ProducerHostedService_StartAsync_Should_CallStartAsync()
        {
            var producerServiceMock = new Mock<IProducerService>();
            producerServiceMock.Setup(x => x.StartAsync(default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var producerHostedService = new ProducerHostedService(producerServiceMock.Object);
            await producerHostedService.StartAsync(default);

            producerServiceMock.Verify();
        }

        [Test]
        public async Task ProducerHostedService_StopAsync_Should_CallStopAsync()
        {
            var producerServiceMock = new Mock<IProducerService>();
            producerServiceMock.Setup(x => x.StopAsync(default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var producerHostedService = new ProducerHostedService(producerServiceMock.Object);
            await producerHostedService.StopAsync(default);

            producerServiceMock.Verify();
        }
    }
}
