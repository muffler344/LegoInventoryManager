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
            var setsResult = await _rebrickableAccessor.SearchLegoSetsBySetID(setID);
            if (!setsResult.IsSuccess) return Error(setsResult.Exception);
            return Success(setsResult.Payload.Results.OrderBy(x => Math.Abs((long)x.SetNumber.Length - setID.Length)).First());
        }

        public async Task<ThemeRebrickable?> ReadThemeRebrickable(int themeID)
        {
            //TODO Convert to Result
            return await _rebrickableAccessor.GetThemeDetailsByThemeID(themeID) ?? null;
        }

        public async Task<PriceGuide?> ReadPriceGuideBySetID(string setID)
        {
            //TODO Rework BrickLinkAccessor
            return await _brickLinkAccessor.Client.GetPriceGuideAsync(ItemType.Set, setID, priceGuideType: PriceGuideType.Stock, condition: Condition.New) ?? null;
        }
    }
}
