using System.Collections.Generic;

namespace DepthQuotesProducer.Binance
{
    public class Quote
    {
#nullable disable
        public Quote()
        {
        }
#nullable enable

        public Quote (long lastUpdateId, IEnumerable<decimal[]> bids, IEnumerable<decimal[]> asks)
        {
            LastUpdateId = lastUpdateId;
            Bids = bids;
            Asks = asks;
        }

        public long LastUpdateId { get; set; }

        /// <summary> Orders for buy. There are 2 numbers, first is price, second is quantity. </summary>
        public IEnumerable<decimal[]> Bids { get; set; }

        /// <summary> Orders for sell. There are 2 numbers, first is price, second is quantity. </summary>
        public IEnumerable<decimal[]> Asks { get; set; }
    }
}
