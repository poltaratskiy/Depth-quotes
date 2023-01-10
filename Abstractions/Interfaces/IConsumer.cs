using Abstractions.CommonObjects;
using System;

namespace Abstractions.Interfaces
{
    public interface IConsumer
    {
        /// <summary> Event related to obtaining quotes. </summary>
        public event EventHandler<QuoteReceivedEventArgs>? QuoteReceived;
    }
}
