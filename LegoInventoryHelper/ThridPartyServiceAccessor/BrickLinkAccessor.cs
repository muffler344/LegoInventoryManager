using BricklinkSharp.Client;
using Microsoft.Extensions.Configuration;
using ThridPartyServiceAccessor.Entities;
using static ThridPartyServiceAccessor.Entities.ResultHelper;

namespace ThridPartyServiceAccessor
{
    public class BrickLinkAccessor : IDisposable
    {
        private readonly IBricklinkClient _bricklinkClient;
        public BrickLinkAccessor(IConfiguration configuration)
        {
            BricklinkClientConfiguration.Instance.TokenValue = configuration["Bricklink:TokenValue"];
            BricklinkClientConfiguration.Instance.TokenSecret = configuration["Bricklink:TokenSecret"];
            BricklinkClientConfiguration.Instance.ConsumerKey = configuration["Bricklink:ConsumerKey"];
            BricklinkClientConfiguration.Instance.ConsumerSecret = configuration["Bricklink:ConsumerSecret"];
            _bricklinkClient = BricklinkClientFactory.Build();
        }

        public void Dispose()
        {
            _bricklinkClient?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<Result<PriceGuide, string>> GetPriceGuideAsync(string setID)
        {
            var result = await _bricklinkClient.GetPriceGuideAsync(ItemType.Set, setID, priceGuideType: PriceGuideType.Stock, condition: Condition.New);
            return (result == null)
                ? Error($"Priceguide from Set {setID} could not be read.")
                : Success(result);
        }
    }
}
