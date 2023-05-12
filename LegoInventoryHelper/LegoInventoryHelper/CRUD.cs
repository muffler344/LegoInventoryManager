using LegoInventoryHelper.DatabaseContext;
using LegoInventoryHelper.DatabaseContext.Entities;
using LegoInventoryHelper.Extensions;
using LegoInventoryHelper.Models;
using Microsoft.EntityFrameworkCore;
using ThridPartyServiceAccessor.Entities;
using ThridPartyServiceAccessor.Interfaces;
using static ThridPartyServiceAccessor.Entities.ResultHelper;

namespace LegoInventoryHelper
{
    public class CRUD
    {
        private readonly ILegoSetDataRetriver _legoSetDataRetriver;
        private readonly LegoInventoryContext _legoInventoryContext;

        private IQueryable<LegoInventoryItem> AllLegoInventoryItems => _legoInventoryContext.LegoInventoryItems.Include(lii => lii.Prices).Include(lii => lii.Theme).AsNoTracking().AsQueryable();

        public CRUD(
            ILegoSetDataRetriver legoSetDataRetriver,
            LegoInventoryContext legoInventoryContext)
        {
            _legoSetDataRetriver = legoSetDataRetriver;
            _legoInventoryContext = legoInventoryContext;
        }

        public async Task<Result<LegoInventoryItem, string>> CreateLegoInventoryItem(CreateInventoryItem item)
        {
            var setResult = await _legoSetDataRetriver.ReadLegoDataSet(item.SetID);
            if (!setResult.IsSuccess) return Error($"Set with Set ID {item.SetID} could not be identified.");
            var themeResult = await CreateThemes(setResult.Payload);
            if (!themeResult.IsSuccess)
                return Error(themeResult.Exception);
            var pricesToAdd = await DeterminePrices(setResult.Payload);
            var legoInventoryItem = new LegoInventoryItem(item, setResult.Payload, themeResult.Payload, pricesToAdd);
            await _legoInventoryContext.AddAsync(legoInventoryItem);
            return await SaveChangesCheckIfSuccessfullAsync() ? Success(legoInventoryItem) : Error("Something went wrong while saving the Set into the database.");
        }

        //Read
        public async Task<List<LegoInventoryItem>> ReadAllLegoInventoryItems() => await AllLegoInventoryItems.ToListAsync();


        //TODO Add UpdatePrice when Item is read only in the singular call 
        public async Task<LegoInventoryItem?> ReadByID(int id) => await AllLegoInventoryItems.FirstOrDefaultAsync(lii => lii.ID == id);

        public async Task<LegoInventoryItem?> ReadBySetID(string setID) => await AllLegoInventoryItems.FirstOrDefaultAsync(lii => lii.SetID == setID);


        //Update
        public async Task<bool> Update(LegoInventoryItem lii)
        {
            var liiFromDB = await _legoInventoryContext.LegoInventoryItems.FindAsync(lii.ID);
            if (liiFromDB == null) return false;
            LegoInventoryItem.Update(liiFromDB, lii);
            _legoInventoryContext.Update(liiFromDB);
            return await SaveChangesCheckIfSuccessfullAsync();
        }

        public async Task<bool> UpdatePrices(int id)
        {
            var lii = await _legoInventoryContext.LegoInventoryItems.FindAsync(id);
            if (lii == null) return false;
            var priceFromBricklink = await _legoSetDataRetriver.ReadPriceGuideBySetID(lii.SetID);
            if (priceFromBricklink == null) return false;
            _legoInventoryContext.Attach(lii);
            lii.Prices.Add(priceFromBricklink.ToPrice(lii.SetID));
            _legoInventoryContext.Update(lii);
            return await SaveChangesCheckIfSuccessfullAsync();
        }

        //Delete
        public async Task<bool> Delete(int id)
        {
            var lii = await _legoInventoryContext.LegoInventoryItems.Include(lii => lii.Prices).FirstAsync(lii => lii.ID == id);
            if (lii == null) return false;
            _legoInventoryContext.Remove(lii);
            return await SaveChangesCheckIfSuccessfullAsync();
        }

        //Privates
        public async Task<Result<Theme, string>> CreateThemes(Set set)
        {
            var theme = await _legoInventoryContext.Themes.FirstOrDefaultAsync(x => x.ThemeID == set.ThemeID);
            if (theme != null)
                _legoInventoryContext.Attach(theme);
            else theme = (await
                    _legoSetDataRetriver.ReadThemeRebrickable(set.ThemeID))?.ToTheme();
            if (theme == null) return Error("No Theme found.");
            await CreateParentTheme(theme);
            return Success(theme);
        }
        private async Task CreateParentTheme(Theme theme)
        {
            var parentThemeID = theme.ParentID;
            if (parentThemeID != null)
            {
                var parentTheme = await _legoInventoryContext.Themes.FirstOrDefaultAsync(t => t.ThemeID == parentThemeID);
                if (parentTheme != null)
                    return;
                else
                     parentTheme = (await _legoSetDataRetriver.ReadThemeRebrickable(parentThemeID.Value))?.ToTheme();
                if (parentTheme != null)
                {
                    _legoInventoryContext.Themes.Add(parentTheme);
                    await CreateParentTheme(parentTheme);
                }   
            }
        }
        private async Task<List<Price>> DeterminePrices(Set set)
        {
            var pricesToAdd = new List<Price>();
            var existingPrices = await _legoInventoryContext.Prices.Where(p => p.SetID == set.SetNumber).ToListAsync();
            _legoInventoryContext.AttachRange(existingPrices);
            pricesToAdd.AddRange(existingPrices);
            var priceFromBricklink = await _legoSetDataRetriver.ReadPriceGuideBySetID(set.SetNumber);
            if (priceFromBricklink != null)
                pricesToAdd.Add(priceFromBricklink.ToPrice(set.SetNumber));
            return pricesToAdd;
        }
        private async Task<bool> SaveChangesCheckIfSuccessfullAsync()
        {
            var changedRows = await _legoInventoryContext.SaveChangesAsync();
            if (changedRows < 1) return false;
            return true;
        }
    }
}