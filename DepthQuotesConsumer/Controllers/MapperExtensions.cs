using System.Linq;

namespace DepthQuotesConsumer.Controllers
{
    internal static class MapperExtensions
    {
        internal static Abstractions.WebApi.Quote ToWebApiQuote(this Abstractions.ProducerConsumerDto.Quote quote)
        {
            var bids = quote.Bids.Select(x => new Abstractions.WebApi.Level(x.Price, x.Quantity));
            var asks = quote.Asks.Select(x => new Abstractions.WebApi.Level(x.Price, x.Quantity));

            return new Abstractions.WebApi.Quote($"public.depth.{quote.Symbol.ToLower()}", bids, asks);
        }
    }
}
