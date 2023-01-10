using DepthQuotesConsumer.Config;
using Microsoft.Extensions.Logging;
using Moq;
using NATS.Client;
using NUnit.Framework;
using System;

namespace DepthQuotesConsumer.Tests
{
    internal class NatsConsumerTests
    {
        [Test]
        public void CreateConsumer_Should_ConnectAndSubscribe()
        {
            var asyncSubscriptionMock = new Mock<IAsyncSubscription>();
            asyncSubscriptionMock.Setup(x => x.Start())
                .Verifiable();

            var connectionMock = new Mock<IConnection>();
            connectionMock.Setup(x => x.SubscribeAsync("channel", It.IsAny<EventHandler<MsgHandlerEventArgs>>()))
                .Returns(asyncSubscriptionMock.Object)
                .Verifiable();

            var connectionFactoryMock = new Mock<IConnectionFactory>();
            connectionFactoryMock.Setup(x => x.CreateConnection("url"))
                .Returns(connectionMock.Object)
                .Verifiable();


            var logger = new Logger<NatsConsumer>(new LoggerFactory());
            var options = Microsoft.Extensions.Options.Options.Create(new NatsConfiguration() { Channel = "channel", Url = "url" });
            var consumer = new NatsConsumer(logger, connectionFactoryMock.Object, options);

            connectionFactoryMock.Verify();
            connectionMock.Verify();
            asyncSubscriptionMock.Verify();
        }
    }
}