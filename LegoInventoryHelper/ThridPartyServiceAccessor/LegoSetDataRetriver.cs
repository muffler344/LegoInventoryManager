using BricklinkSharp.Client;
using ThridPartyServiceAccessor.Entities;
using ThridPartyServiceAccessor.Interfaces;
using static ThridPartyServiceAccessor.Entities.ResultHelper;

namespace ThridPartyServiceAccessor
{
    public class LegoSetDataRetriver : ILegoSetDataRetriver, IDisposable
    {
        private readonly BrickLinkAccessor _brickLinkAccessor;
        private readonly RebrickableAccessor _rebrickableAccessor;

        public LegoSetDataRetriver(BrickLinkAccessor brickLinkAccessor, RebrickableAccessor rebrickableAccessor)
        {
            _brickLinkAccessor = brickLinkAccessor;
            _rebrickableAccessor = rebrickableAccessor;
        }

        public void Dispose()
        {
            _brickLinkAccessor.Dispose();
            _rebrickableAccessor.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<Result<Set, string>> ReadLegoDataSet(string setID)
        {
            var result = await _rebrickableAccessor.SearchLegoSetsBySetID(setID);
            if (!result.IsSuccess) return Error(result.Exception);
            return Success(result.Payload.Results.OrderBy(x => Math.Abs((long)x.SetNumber.Length - setID.Length)).First());
        }

        public async Task<Result<ThemeRebrickable, string>> ReadThemeRebrickable(int themeID)
        {
            var result = await _rebrickableAccessor.GetThemeDetailsByThemeID(themeID);
            if (!result.IsSuccess) return Error(result.Exception);
            return Success(result.Payload);
        }

        public async Task<Result<PriceGuide, string>> ReadPriceGuideBySetID(string setID)
        {
            var result = await _brickLinkAccessor.GetPriceGuideAsync(setID);
            if (!result.IsSuccess) return Error(result.Exception);
            return Success(result.Payload);
        }
    }
}
