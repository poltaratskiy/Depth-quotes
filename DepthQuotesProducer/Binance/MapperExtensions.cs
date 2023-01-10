using System.Linq;

namespace DepthQuotesProducer.Binance
{
    internal static class MapperExtensions
    {
        internal static Abstractions.CommonObjects.Quote ToAbstractionsQuote(this Quote binanceQuote, string symbol)
        {
            // Binance sends an array for level - first is price, second is quantity
            var bids = binanceQuote.Bids.Select(x => new Abstractions.CommonObjects.Level(x[0], x[1]));
            var asks = binanceQuote.Asks.Select(x => new Abstractions.CommonObjects.Level(x[0], x[1]));

            return new Abstractions.CommonObjects.Quote(symbol, bids, asks);
        }
    }
}
