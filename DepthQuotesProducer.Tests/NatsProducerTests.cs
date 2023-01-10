using Abstractions.ProducerConsumerDto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NATS.Client;
using NUnit.Framework;
using System.Threading.Tasks;

namespace DepthQuotesProducer.Tests
{
    internal class NatsProducerTests
    {
        private readonly ILogger<NatsProducer.NatsProducer> _logger = new Logger<NatsProducer.NatsProducer>(new LoggerFactory());
        private readonly IOptions<NatsProducer.NatsConfiguration> _options = 
            Microsoft.Extensions.Options.Options.Create(new NatsProducer.NatsConfiguration() { Channel = "channel", Url = "url" });

        [Test]
        public async Task NatsProducer_Connect_Should_CreateConnection()
        {
            var connectionFactoryMock = new Mock<IConnectionFactory>();
            var returnedConnection = Mock.Of<IConnection>();
            connectionFactoryMock
                .Setup(x => x.CreateConnection("url"))
                .Returns(returnedConnection)
                .Verifiable();

            var producer = new NatsProducer.NatsProducer(_logger, connectionFactoryMock.Object, _options);
            await producer.ConnectAsync(default);

            // check if method CreateConnection was called
            connectionFactoryMock.VerifyAll();
            connectionFactoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task NatsProducer_SendQuote_Should_CallPublish()
        {
            var connectionMock = new Mock<IConnection>();
            connectionMock
                .Setup(x => x.Publish("channel", It.IsAny<byte[]>()))
                .Verifiable();

            var connectionFactoryMock = new Mock<IConnectionFactory>();
            connectionFactoryMock
                .Setup(x => x.CreateConnection("url"))
                .Returns(connectionMock.Object);

            var producer = new NatsProducer.NatsProducer(_logger, connectionFactoryMock.Object, _options);
            await producer.ConnectAsync(default);

            var bids = new Level[]
            {
                new Level(12.1M, 12.5M),
            };

            var asks = new Level[]
            {
                new Level(10.1M, 10.5M),
            };
            await producer.SendQuoteAsync(new Quote("BNBBTC", bids, asks));

            // check if method Publish was called
            connectionMock.VerifyAll();
            connectionMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task NatsProducer_CloseConection_Should_CallCloseAndDispose()
        {
            var connectionMock = new Mock<IConnection>();
            connectionMock
                .Setup(x => x.Close())
                .Verifiable();

            connectionMock
                .Setup(x => x.Dispose())
                .Verifiable();

            var connectionFactoryMock = new Mock<IConnectionFactory>();
            connectionFactoryMock
                .Setup(x => x.CreateConnection("url"))
                .Returns(connectionMock.Object);

            var producer = new NatsProducer.NatsProducer(_logger, connectionFactoryMock.Object, _options);
            await producer.ConnectAsync(default);
            await producer.CloseConnectionAsync(default);

            connectionMock.VerifyAll();
            connectionMock.VerifyNoOtherCalls();
        }
    }
}
