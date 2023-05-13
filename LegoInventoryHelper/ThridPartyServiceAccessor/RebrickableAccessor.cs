using Microsoft.Extensions.Configuration;
using ThridPartyServiceAccessor.Entities;
using static ThridPartyServiceAccessor.Entities.ResultHelper;

namespace ThridPartyServiceAccessor
{
    public class RebrickableAccessor : IDisposable
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;
        public RebrickableAccessor(IConfiguration configuration)
        {
            _client = new HttpClient();
            _apiKey = configuration["Rebrickalbe:ApiKey"];
        }

        public async Task<Result<Sets, string>> SearchLegoSetsBySetID(string setID, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync($"https://rebrickable.com/api/v3/lego/sets/?search={setID}&key={_apiKey}", cancellationToken);
            return (response == null || !response.IsSuccessStatusCode) 
                ? Error($"Set with Set ID {setID} could not be identified.") 
                : Success(await response.Content.ReadAsAsync<Sets>(cancellationToken));
        }

        public async Task<Result<ThemeRebrickable, string>> GetThemeDetailsByThemeID(int themeID, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync($"https://rebrickable.com/api/v3/lego/themes/{themeID}/?key={_apiKey}", cancellationToken);
            return (response == null || !response.IsSuccessStatusCode)
                ? Error($"Theme with Theme ID {themeID} could not be identified.")
                : Success(await response.Content.ReadAsAsync<ThemeRebrickable>(cancellationToken));
        }

        public void Dispose()
        {
            _client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
