using BricklinkSharp.Client;
using ThridPartyServiceAccessor.Entities;

namespace ThridPartyServiceAccessor.Interfaces
{
    public interface ILegoSetDataRetriver
    {
        void Dispose();
        Task<Result<Set, string>> ReadLegoDataSet(string setID);
        Task<Result<PriceGuide, string>> ReadPriceGuideBySetID(string setID);
        Task<Result<ThemeRebrickable, string>> ReadThemeRebrickable(int themeID);
    }
}