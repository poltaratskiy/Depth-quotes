using System;

namespace Abstractions.ProducerConsumerDto
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
