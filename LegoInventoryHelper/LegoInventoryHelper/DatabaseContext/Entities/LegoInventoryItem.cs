﻿using DocumentFormat.OpenXml.Office2019.Excel.RichData2;
using LegoInventoryHelper.Models;
using ThridPartyServiceAccessor.Entities;

namespace LegoInventoryHelper.DatabaseContext.Entities
{
    public class LegoInventoryItem
    {
        /// <summary>
        /// Database ID
        /// </summary>
        public int ID { get; set; }
        public string Name { get; set; }
        public int NumberOfParts { get; set; }
        public string ImageURL { get; set; }
        /// <summary>
        /// Set Number 
        /// </summary>
        public string SetID { get; set; }

        public Theme Theme { get; set; }
        public int YearOfRelease { get; set; }
        public double PriceBought { get; set; }
        public double PriceSold { get; set; }
        public bool IsSellable { get; set; }
        public List<Price> Prices { get; set; } = new();
        public LegoInventoryItem() { }
        public LegoInventoryItem(CreateInventoryItem item, Set set, Theme theme, List<Price> prices)
        {
            Name = set.Name;
            NumberOfParts = set.NumberOfParts;
            ImageURL = set.SetImageURL;
            SetID = set.SetNumber;
            Theme = theme;
            YearOfRelease = set.YearOfRelease;
            PriceBought = item.PriceBought;
            IsSellable = item.IsSellable;
            Prices.AddRange(prices);
        }

        public bool IsTheLatestPriceOutdated(int dayCounter)
        {
            return (Prices.Count < 1) || (DateTime.Now - Prices.Max(x => x.RequestDate)).Days >= dayCounter;
        }

        public static LegoInventoryItem Update(LegoInventoryItem toUpdateLii, LegoInventoryItem liiWithUpdates)
        {
            toUpdateLii.PriceBought = liiWithUpdates.PriceBought;
            toUpdateLii.PriceSold = liiWithUpdates.PriceSold;
            toUpdateLii.IsSellable = liiWithUpdates.IsSellable;
            return toUpdateLii;
        }
    }
}