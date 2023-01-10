using Abstractions.CommonObjects;
using DepthQuotesProducer.Binance;
using NUnit.Framework;
using Shouldly;
using System.Linq;

namespace DepthQuotesProducer.Tests.MappingFromBinance
{
    internal class BinanceMappingTests
    {
        [Test]
        public void MappingFromBinanceToCommonFormat_Should_BeEqual()
        {
            var binanceBids = new[]
            {
                new decimal[] { 0.01618300M, 0.36000000M },
                new decimal[] { 0.01618200M, 2.88900000M },
                new decimal[] { 0.01617600M, 1.39600000M },
                new decimal[] { 0.01617400M, 2.42500000M },
            };

            var binanceAsks = new[]
            {
                new decimal[] { 0.01618400M, 9.74400000M },
                new decimal[] { 0.01618700M, 9.92300000M },
                new decimal[] { 0.01618800M, 32.57300000M },
                new decimal[] { 0.01618900M, 0.37600000M },
            };

            var binanceQuote = new Binance.Quote(123, binanceBids, binanceAsks);

            var expectedBids = new Level[]
            {
                new Level(0.01618300M, 0.36000000M),
                new Level(0.01618200M, 2.88900000M),
                new Level(0.01617600M, 1.39600000M),
                new Level(0.01617400M, 2.42500000M),
            };

            var expectedAsks = new Level[]
            {
                new Level(0.01618400M, 9.74400000M),
                new Level(0.01618700M, 9.92300000M),
                new Level(0.01618800M, 32.57300000M),
                new Level(0.01618900M, 0.37600000M),
            };

            var expectedQuote = new Abstractions.CommonObjects.Quote("BNBBTC", expectedBids, expectedAsks);
            var actualQuote = binanceQuote.ToAbstractionsQuote("BNBBTC");

            QuotesAreEqual(actualQuote, expectedQuote).ShouldBeTrue();
        }

        private bool QuotesAreEqual(Abstractions.CommonObjects.Quote? actualQuote, Abstractions.CommonObjects.Quote? expectedQuote)
        {
            if (actualQuote == null || expectedQuote == null)
            {
                return false;
            }

            if (actualQuote.Symbol != expectedQuote.Symbol)
            {
                return false;
            }

            var levelComparer = new LevelEqualityComparer();

            if (!actualQuote.Bids.SequenceEqual(expectedQuote.Bids, levelComparer) || !actualQuote.Asks.SequenceEqual(expectedQuote.Asks, levelComparer))
            {
                return false;
            }

            return true;
        }
    }
}