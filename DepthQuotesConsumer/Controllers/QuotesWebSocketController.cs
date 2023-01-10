using Abstractions.ProducerConsumerDto;
using Abstractions.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace DepthQuotesConsumer.Controllers
{
    public class QuotesWebSocketController : ControllerBase, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IConsumer _consumer;
        private WebSocket? _webSocket;
        private string? _symbol; // we can use field for incoming argument because Controller's lifecycle is scoped so for every request will be allocated own instance of Controller.

        public QuotesWebSocketController(ILogger<QuotesWebSocketController> logger, IConsumer consumer)
        {
            _logger = logger;
            _consumer = consumer;
        }

        /// <summary> Get depth quotes in WebSocket connection. </summary>
        /// <param name="symbol"> Symbol in any case. </param>
        [Route("/ws/public.depth.{symbol}")]
        public async Task GetQuotes(string symbol)
        {
            _symbol = symbol.ToUpper();
            _logger.LogInformation("WS request received, symbol: {0}", _symbol);

            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                _webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                _consumer.QuoteReceived += QuoteReceived;
                while(!HttpContext.RequestAborted.IsCancellationRequested)
                {
                    // keep connection alive until it is closed
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private void QuoteReceived(object? sender, QuoteReceivedEventArgs args)
        {
            if (args.Quote.Symbol == _symbol)
            {
                var task = Task.Run(() =>
                {
                    // Explanation: Producer serializes Quotes to json, consumer obtains and deserializes then serializes again.
                    // Producer could send it in json format and consumer could send it as is but in real projects it is highly likely to make convertations, add logic and etc.
                    var webApiQuotes = args.Quote.ToWebApiQuote();
                    var serializedBytes = JsonSerializer.SerializeToUtf8Bytes(webApiQuotes);

                    // _webSocket is marked by ! because it has been already created before subscribing the event
                    return _webSocket!.SendAsync(serializedBytes, WebSocketMessageType.Text, true, default);
                });

                task.ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _consumer.QuoteReceived -= QuoteReceived;
            _webSocket?.Dispose();
        }
    }
}
