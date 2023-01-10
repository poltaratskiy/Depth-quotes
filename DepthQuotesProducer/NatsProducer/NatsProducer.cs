using Abstractions.CommonObjects;
using Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DepthQuotesProducer.NatsProducer
{
    public class NatsProducer : IProducer, IAsyncDisposable
    {
        private readonly IConnectionFactory _connectionFactory; // ConnectionFactory carried out to DI for tests instead of creating it in constructor
        private readonly ILogger _logger;
        private readonly NatsConfiguration _natsConfiguration;

        // NatsProducer is declared as singleton and is used only in hosted service which runs ConnectAsync method only once
        // so it is no need to take care about using static fields and lock expression
        private IConnection? _connection; 

        public NatsProducer(ILogger<NatsProducer> logger, IConnectionFactory connectionFactory, IOptions<NatsConfiguration> natsOptions)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _natsConfiguration = natsOptions.Value;
        }

        public Task ConnectAsync(CancellationToken cancellationToken)
        {
            _connection = _connectionFactory.CreateConnection(_natsConfiguration.Url);
            _logger.LogInformation("Connected to Nats");
            return Task.CompletedTask;
        }

        public Task SendQuoteAsync(Quote quote, CancellationToken cancellationToken = default)
        {
            if (_connection != null)
            {
                var bytes = JsonSerializer.SerializeToUtf8Bytes(quote);
                _connection.Publish(_natsConfiguration.Channel, bytes);
            }

            return Task.CompletedTask;
        }

        public Task CloseConnectionAsync(CancellationToken cancellationToken = default)
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }

            _logger.LogInformation("Connection to Nats closed");
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await CloseConnectionAsync(default);
        }
    }
}
