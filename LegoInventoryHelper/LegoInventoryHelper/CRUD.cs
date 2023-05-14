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
            var set = await _legoSetDataRetriver.ReadLegoDataSet(item.SetID);
            if (!set.IsSuccess) return Error($"Set with Set ID {item.SetID} could not be identified.");
            var theme = await CreateThemes(set.Payload);
            if (!theme.IsSuccess) return Error(theme.Exception);
            var prices = await DeterminePrices(set.Payload);
            var legoInventoryItem = new LegoInventoryItem(item, set.Payload, theme.Payload, prices.IsSuccess ? prices.Payload : new());
            await _legoInventoryContext.AddAsync(legoInventoryItem);
            return await SaveChangesCheckIfSuccessfullAsync(legoInventoryItem, "Something went wrong while saving prices into the database.");
        }

        //Read
        public async Task<Result<List<LegoInventoryItem>, string>> ReadAllLegoInventoryItems()
        {
            var result = await AllLegoInventoryItems.ToListAsync();
            if (result.Count < 1 || result == null) return Error("There are no Sets in the Databas.");
            return Success(result);
        }

        //TODO Add UpdatePrice when Item is read only in the singular call 
        public async Task<LegoInventoryItem?> ReadByID(int id) => await AllLegoInventoryItems.FirstOrDefaultAsync(lii => lii.ID == id);

        public async Task<LegoInventoryItem?> ReadBySetID(string setID) => await AllLegoInventoryItems.FirstOrDefaultAsync(lii => lii.SetID == setID);


        //Update
        public async Task<Result<bool, string>> Update(LegoInventoryItem lii)
        {
            var liiFromDB = await _legoInventoryContext.LegoInventoryItems.FindAsync(lii.ID);
            if (liiFromDB == null) return Error($"Update Set with Set ID {lii.SetID} failed.");
            LegoInventoryItem.Update(liiFromDB, lii);
            _legoInventoryContext.Update(liiFromDB);
            return await SaveChangesCheckIfSuccessfullAsync(true, "Something went wrong while updating a Set.");
        }

        public async Task<Result<bool, string>> UpdatePrices(int id, int dayCounter = 1)
        {
            var lii = await _legoInventoryContext.LegoInventoryItems.FindAsync(id);
            if (lii == null) return Error($"No Set found.");
            if (!lii.IsTheLatestPriceOutdated(dayCounter)) return Error("Prices are up to Date.");
            var result = await _legoSetDataRetriver.ReadPriceGuideBySetID(lii.SetID);
            if (!result.IsSuccess) return Error($"No prices were not found.");
            _legoInventoryContext.Attach(lii);
            lii.Prices.Add(result.Payload.ToPrice(lii.SetID));
            _legoInventoryContext.Update(lii);
            return await SaveChangesCheckIfSuccessfullAsync(true ,"Something went wrong while saving prices into the database.");
        }

        //Delete
        public async Task<Result<bool, string>> Delete(int id)
        {
            var lii = await _legoInventoryContext.LegoInventoryItems.Include(lii => lii.Prices).FirstAsync(lii => lii.ID == id);
            if (lii == null) return Error($"Deleting Set with Set ID {id} failed.");
            _legoInventoryContext.Remove(lii);
            return await SaveChangesCheckIfSuccessfullAsync(true, "Something went wrong while deleting a Set.");
        }

        //Privates
        private async Task<Result<Theme, string>> CreateThemes(Set set)
        {
            var theme = await _legoInventoryContext.Themes.FirstOrDefaultAsync(x => x.ThemeID == set.ThemeID);
            if (theme != null) _legoInventoryContext.Attach(theme);
            else
            {
                var result = await _legoSetDataRetriver.ReadThemeRebrickable(set.ThemeID);
                if (result.IsSuccess) theme = result.Payload.ToTheme();
                else return Error(result.Exception);
            }

            await CreateParentTheme(theme);
            return Success(theme);
        }
        private async Task CreateParentTheme(Theme theme)
        {
            if (!theme.ParentID.HasValue) return;
            var parentTheme = await _legoInventoryContext.Themes.FirstOrDefaultAsync(t => t.ThemeID == theme.ParentID);
            if (parentTheme != null) return;
            var result = await _legoSetDataRetriver.ReadThemeRebrickable((int)theme.ParentID);
            if (!result.IsSuccess) return;
            parentTheme = result.Payload.ToTheme();
            _legoInventoryContext.Themes.Add(parentTheme);
            await CreateParentTheme(parentTheme);
        }
        private async Task<Result<List<Price>, string>> DeterminePrices(Set set)
        {
            var pricesToAdd = new List<Price>();
            var existingPrices = await _legoInventoryContext.Prices.Where(p => p.SetID == set.SetNumber).ToListAsync();
            _legoInventoryContext.AttachRange(existingPrices);
            pricesToAdd.AddRange(existingPrices);
            var result = await _legoSetDataRetriver.ReadPriceGuideBySetID(set.SetNumber);
            if (!result.IsSuccess) return Error($"No prices for {set.SetNumber} were found.");
            pricesToAdd.Add(result.Payload.ToPrice(set.SetNumber));
            return Success(pricesToAdd);
        }

        private async Task<Result<T, E>> SaveChangesCheckIfSuccessfullAsync<T, E>(T payload, E exception)
        {
            return (await _legoInventoryContext.SaveChangesAsync()) < 1 ? Error(exception) : Success(payload);
        }
    }
}