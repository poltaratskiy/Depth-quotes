using System.Collections.Generic;

namespace Abstractions.WebApi
{
    public class Quote
    {
        public Quote (string channel, IEnumerable<Level> bids, IEnumerable<Level> asks)
        {
            Channel = channel;
            Bids = bids;
            Asks = asks;
        }

        /// <summary> Channel. </summary>
        public string Channel { get; }

        /// <summary> Orders for buy. </summary>
        public IEnumerable<Level> Bids { get; }

        /// <summary> Orders for sell. </summary>
        public IEnumerable<Level> Asks { get; }
    }
}
