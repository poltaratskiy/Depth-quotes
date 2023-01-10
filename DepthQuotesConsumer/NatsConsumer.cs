using Abstractions.ProducerConsumerDto;
using Abstractions.Interfaces;
using DepthQuotesConsumer.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;
using System;
using System.Text.Json;

namespace DepthQuotesConsumer
{
    public class NatsConsumer : IConsumer, IDisposable
    {
        /* First I wanted to use _connection and _subscription as static fields and use lock construction to ensure it creates only once
         * but NatsConsumer is declared as singleton in Startup configuration so it is no nessessary to use lock. */
        private readonly IConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly IAsyncSubscription _subscription;
        private readonly ILogger _logger;
        private readonly NatsConfiguration _natsConfiguration;

        public NatsConsumer(ILogger<NatsConsumer> logger, IConnectionFactory connectionFactory, IOptions<NatsConfiguration> natsConfigurationOptions)
        {
            _logger = logger;
            _natsConfiguration = natsConfigurationOptions.Value;

            _connectionFactory = connectionFactory;
            _connection = _connectionFactory.CreateConnection(_natsConfiguration.Url);
            _logger.LogInformation("Connected to Nats");

            _subscription = _connection.SubscribeAsync(_natsConfiguration.Channel, (sender, args) =>
            {
                var quote = JsonSerializer.Deserialize<Quote>(args.Message.Data);

                if (quote != null)
                {
                    OnQuoteReceived(new QuoteReceivedEventArgs(quote));
                }
            });
            _subscription.Start();
            _logger.LogInformation("Subscribed to Nats");
        }

        public event EventHandler<QuoteReceivedEventArgs>? QuoteReceived;

        protected virtual void OnQuoteReceived(QuoteReceivedEventArgs quoteReceivedEventArgs)
        {
            QuoteReceived?.Invoke(this, quoteReceivedEventArgs);
        }

        public void Dispose()
        {
            _subscription?.Unsubscribe();
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }

            _logger.LogInformation("Connection to Nats closed");
        }
    }
}
