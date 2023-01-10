using Abstractions.CommonObjects;
using Abstractions.Interfaces;
using DepthQuotesProducer.AppConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DepthQuotesProducer.Binance
{
    public class BinanceClient : IExchangeClient, IAsyncDisposable
    {
        private const string urlTemplate = "wss://stream.binance.com:9443/ws/<SYMBOL>@depth20@100ms"; // constant could be carried to config as an url template to set up depth or interval for example

        private readonly ILogger _logger;
        private readonly SymbolsConfiguration _symbolsConfiguration;
        private readonly ClientWebSocket client;

        public BinanceClient(ILogger<BinanceClient> logger, IOptions<SymbolsConfiguration> symbolsConfigurationOptions)
        {
            _logger = logger;
            _symbolsConfiguration = symbolsConfigurationOptions.Value;
            client = new ClientWebSocket();
        }

        public event EventHandler<QuoteReceivedEventArgs>? QuoteReceived;

        protected virtual void OnQuoteReceived(QuoteReceivedEventArgs quoteReceivedEventArgs)
        {
            QuoteReceived?.Invoke(this, quoteReceivedEventArgs);
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Symbol in config is {0}", _symbolsConfiguration.Symbol);
            var url = urlTemplate.Replace("<SYMBOL>", _symbolsConfiguration.Symbol.ToLower());

            await client.ConnectAsync(new Uri(url), cancellationToken);
            _logger.LogInformation("Connected to Binance");
            byte[] buffer = new byte[20*1024]; // 20 kb is enough for one message

            while (client.State == WebSocketState.Open)
            {
                var result = await client.ReceiveAsync(buffer, cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
                }
                else
                {
                    var receivedJson = Encoding.UTF8.GetString(buffer);

                    // I spent 4 hours to get why Binance sends invalid jsons, in fact method ReceiveAsync doesn't clean buffer and it leads to extra symbols at the end
                    Array.Clear(buffer, 0, buffer.Length);

                    try
                    {
                        var binanceQuote = JsonConvert.DeserializeObject<Quote>(receivedJson);

                        if (binanceQuote != null)
                        {
                            var args = new QuoteReceivedEventArgs(binanceQuote.ToAbstractionsQuote(_symbolsConfiguration.Symbol));
                            OnQuoteReceived(args);
                        }
                    }
                    catch (JsonReaderException)
                    {
                        _logger.LogDebug("Received invalid json, skipped");
                    }
                    
                }
            }
        }

        public async Task CloseConnectionAsync(CancellationToken cancellationToken)
        {
            if (client.State == WebSocketState.Connecting || client.State == WebSocketState.Open)
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
            }

            _logger.LogInformation("Binance connection closed");
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            // use dispose to close connection for situations for example when app is stopped but method CloseConnectionAsync was not called
            if (client != null)
            {
                await CloseConnectionAsync(default);
                client.Dispose();
            }
        }
    }
}
