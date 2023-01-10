using System.Collections.Generic;

namespace Abstractions.CommonObjects
{
    public class Quote
    {
        public Quote (string symbol, IEnumerable<Level> bids, IEnumerable<Level> asks)
        {
            Symbol = symbol;
            Bids = bids;
            Asks = asks;
        }

        /// <summary> Symbol in upper case. </summary>
        public string Symbol { get; }

        /// <summary> Orders for buy. </summary>
        public IEnumerable<Level> Bids { get; }

        /// <summary> Orders for sell. </summary>
        public IEnumerable<Level> Asks { get; }
    }
}
