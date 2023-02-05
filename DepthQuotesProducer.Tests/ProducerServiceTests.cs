using Abstractions.ProducerConsumerDto;
using Abstractions.Interfaces;
using DepthQuotesProducer.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DepthQuotesProducer.Tests
{
    internal class ProducerServiceTests
    {
        private readonly ILogger<ProducerService> _logger = new Logger<ProducerService>(new LoggerFactory());

        [Test]
        public async Task ProducerService_StartAsync_Should_ConnectClientAndProducer()
        {
            var producerMock = new Mock<IProducer>();
            producerMock.Setup(x => x.ConnectAsync(default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            producerMock.Setup(x => x.SendQuoteAsync(It.IsAny<Quote>(), default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var quoteToSend = new Quote("SYMBOL", Enumerable.Empty<Level>(), Enumerable.Empty<Level>());

            var exchangeClientMock = new Mock<IExchangeClient>();
            exchangeClientMock.Setup(x => x.ConnectAsync((q) => producerMock.Object.SendQuoteAsync(q, default), default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            

            var producerService = new ProducerService(_logger, exchangeClientMock.Object, producerMock.Object);
            await producerService.StartAsync(default);

            exchangeClientMock.VerifyAll();

            producerMock.VerifyAll();
            producerMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task ProducerService_StopAsync_Should_StopClientAndProducer()
        {
            var exchangeClientMock = new Mock<IExchangeClient>();
            exchangeClientMock.Setup(x => x.CloseConnectionAsync(default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var producerMock = new Mock<IProducer>();
            producerMock.Setup(x => x.CloseConnectionAsync(default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var producerService = new ProducerService(_logger, exchangeClientMock.Object, producerMock.Object);
            await producerService.StopAsync(default);

            exchangeClientMock.VerifyAll();

            producerMock.VerifyAll();
            producerMock.VerifyNoOtherCalls();
        }
    }
}
