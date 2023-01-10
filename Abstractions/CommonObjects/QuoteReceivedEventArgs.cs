using System;

namespace Abstractions.CommonObjects
{
    public class QuoteReceivedEventArgs : EventArgs
    {
        public QuoteReceivedEventArgs(Quote quote)
        {
            Quote = quote;
        }
        
        public Quote Quote { get; }
    }
}
