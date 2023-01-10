namespace DepthQuotesProducer.NatsProducer
{
    public class NatsConfiguration
    {
#nullable disable
        public NatsConfiguration()
        {
        }
#nullable enable

        public string Url { get; set; }

        public string Channel { get; set; }
    }
}
