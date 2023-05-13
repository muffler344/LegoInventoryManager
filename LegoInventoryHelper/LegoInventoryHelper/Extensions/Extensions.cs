using BricklinkSharp.Client;
using LegoInventoryHelper.DatabaseContext.Entities;
using ThridPartyServiceAccessor.Entities;

namespace LegoInventoryHelper.Extensions
{
    public static class Extensions
    {
        public static Price ToPrice(this PriceGuide priceGuide, string setID) => new(priceGuide, setID);
        public static Theme ToTheme(this ThemeRebrickable themeRebickalbe) => new(themeRebickalbe);
    }
}
