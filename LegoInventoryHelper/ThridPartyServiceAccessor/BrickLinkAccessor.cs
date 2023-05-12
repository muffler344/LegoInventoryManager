using BricklinkSharp.Client;
using Microsoft.Extensions.Configuration;

namespace ThridPartyServiceAccessor
{
    public class BrickLinkAccessor : IDisposable
    {
        public IBricklinkClient Client { get;}
        public BrickLinkAccessor(IConfiguration configuration)
        {
            BricklinkClientConfiguration.Instance.TokenValue = configuration["Bricklink:TokenValue"];
            BricklinkClientConfiguration.Instance.TokenSecret = configuration["Bricklink:TokenSecret"];
            BricklinkClientConfiguration.Instance.ConsumerKey = configuration["Bricklink:ConsumerKey"];
            BricklinkClientConfiguration.Instance.ConsumerSecret = configuration["Bricklink:ConsumerSecret"];
            Client = BricklinkClientFactory.Build();
        }

        public void Dispose()
        {
            Client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
