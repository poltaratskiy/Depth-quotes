using Abstractions.Interfaces;
using Abstractions.ProducerConsumerDto;
using DepthQuotesConsumer.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DepthQuotesConsumer
{
    public class NatsConsumer : IConsumer, IDisposable
    {
        /* First I wanted to use _connection and _subscription as static fields and use lock construction to ensure it creates only once
         * but NatsConsumer is declared as singleton in Startup configuration so it is no nessessary to use lock. */
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private readonly NatsConfiguration _natsConfiguration;
        private IConnection? _connection;
        private IAsyncSubscription? _subscription;

        private bool _disposed;

        public NatsConsumer(ILogger<NatsConsumer> logger, IConnectionFactory connectionFactory, IOptions<NatsConfiguration> natsConfigurationOptions)
        {
            _logger = logger;
            _natsConfiguration = natsConfigurationOptions.Value;
            _connectionFactory = connectionFactory;
        }

        public Task ConnectAsync(Func<Quote, Task> quoteReceived, CancellationToken cancellationToken = default)
        {
            _connection = _connectionFactory.CreateConnection(_natsConfiguration.Url);
            _logger.LogInformation("Connected to Nats");

            _subscription = _connection.SubscribeAsync(_natsConfiguration.Channel, async (sender, args) =>
            {
                var quote = JsonSerializer.Deserialize<Quote>(args.Message.Data);

                if (quote != null)
                {
                    await quoteReceived.Invoke(quote);
                }
            });
            _subscription.Start();
            _logger.LogInformation("Subscribed to Nats");

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task CloseConnectionAsync(CancellationToken cancellationToken = default)
        {
            // this method is idempotent
            _subscription?.Unsubscribe();
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();

                _logger?.LogInformation("Connection to Nats closed");
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);

            // https://learn.microsoft.com/en-us/dotnet/api/system.idisposable.dispose?view=net-7.0
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // dispose managed state (managed objects). No need to dispose managed object because there are object resolved with DI.
            }

            if (_connection != null && _subscription != null)
            {
                Task.Run(() => CloseConnectionAsync().ConfigureAwait(false));
                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.
            }

            _disposed = true;
        }

        // Use C# finalizer syntax for finalization code.
        // This finalizer will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide finalizer in types derived from this class.
        ~NatsConsumer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(disposing: false) is optimal in terms of
            // readability and maintainability.
            Dispose(disposing: false);
        }
    }
}
