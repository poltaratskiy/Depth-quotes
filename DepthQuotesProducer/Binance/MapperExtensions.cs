using System.Linq;

namespace DepthQuotesProducer.Binance
{
    internal static class MapperExtensions
    {
        internal static Abstractions.ProducerConsumerDto.Quote ToAbstractionsQuote(this Quote binanceQuote, string symbol)
        {
            // Binance sends an array for level - first is price, second is quantity
            var bids = binanceQuote.Bids.Select(x => new Abstractions.ProducerConsumerDto.Level(x[0], x[1]));
            var asks = binanceQuote.Asks.Select(x => new Abstractions.ProducerConsumerDto.Level(x[0], x[1]));

            return new Abstractions.ProducerConsumerDto.Quote(symbol, bids, asks);
        }
    }
}
