using Abstractions.ProducerConsumerDto;
using DepthQuotesConsumer.Controllers;
using NUnit.Framework;
using Shouldly;
using System.Linq;

namespace DepthQuotesProducer.Tests.MappingFromBinance
{
    internal class WebApiMappingTests
    {
        [Test]
        public void MappingFromBinanceToCommonFormat_Should_BeEqual()
        {
            var consumerBids = new[]
            {
                new Level(0.01618300M, 0.36000000M),
                new Level(0.01618200M, 2.88900000M),
                new Level(0.01617600M, 1.39600000M),
                new Level(0.01617400M, 2.42500000M),
            };

            var consumerAsks = new[]
            {
                new Level(0.01618400M, 9.74400000M),
                new Level(0.01618700M, 9.92300000M),
                new Level(0.01618800M, 32.57300000M),
                new Level(0.01618900M, 0.37600000M),
            };

            var consumerQuote = new Quote("SYMBOL", consumerBids, consumerAsks);

            var expectedBids = new Abstractions.WebApi.Level[]
            {
                new Abstractions.WebApi.Level(0.01618300M, 0.36000000M),
                new Abstractions.WebApi.Level(0.01618200M, 2.88900000M),
                new Abstractions.WebApi.Level(0.01617600M, 1.39600000M),
                new Abstractions.WebApi.Level(0.01617400M, 2.42500000M),
            };

            var expectedAsks = new Abstractions.WebApi.Level[]
            {
                new Abstractions.WebApi.Level(0.01618400M, 9.74400000M),
                new Abstractions.WebApi.Level(0.01618700M, 9.92300000M),
                new Abstractions.WebApi.Level(0.01618800M, 32.57300000M),
                new Abstractions.WebApi.Level(0.01618900M, 0.37600000M),
            };

            var expectedQuote = new Abstractions.WebApi.Quote("public.depth.symbol", expectedBids, expectedAsks);
            var actualQuote = consumerQuote.ToWebApiQuote();

            QuotesAreEqual(actualQuote, expectedQuote).ShouldBeTrue();
        }

        private bool QuotesAreEqual(Abstractions.WebApi.Quote? actualQuote, Abstractions.WebApi.Quote? expectedQuote)
        {
            if (actualQuote == null || expectedQuote == null)
            {
                return false;
            }

            if (actualQuote.Channel != expectedQuote.Channel)
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