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
            var exchangeClientMock = new Mock<IExchangeClient>();
            exchangeClientMock.Setup(x => x.ConnectAsync(default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var producerMock = new Mock<IProducer>();
            producerMock.Setup(x => x.ConnectAsync(default))
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

        [Test]
        public async Task ProducerService_ExchangeSendsQuote_Should_CallSendQuotesFromProducer()
        {
            var exchangeClientMock = new Mock<IExchangeClient>();
            exchangeClientMock.Setup(x => x.ConnectAsync(default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var producerMock = new Mock<IProducer>();
            producerMock.Setup(x => x.ConnectAsync(default))
                .Returns(Task.CompletedTask);

            Quote? sentQuote = null;
            producerMock.Setup(x => x.SendQuoteAsync(It.IsAny<Quote>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback<Quote, CancellationToken>((q, c) => sentQuote = q)
                .Verifiable();

            var producerService = new ProducerService(_logger, exchangeClientMock.Object, producerMock.Object);
            await producerService.StartAsync(default);

            var quoteEventArgs = new QuoteReceivedEventArgs(new Quote("BNBBTC", Enumerable.Empty<Level>(), Enumerable.Empty<Level>()));
            exchangeClientMock.Raise(x => x.QuoteReceived += null, quoteEventArgs);

            producerMock.Verify();
            sentQuote.ShouldNotBeNull();
            sentQuote.ShouldSatisfyAllConditions(x =>
            {
                x.Symbol.ShouldBe("BNBBTC");
                x.Bids.ShouldBeEmpty();
                x.Asks.ShouldBeEmpty();
            });
        }
    }
}
