using BricklinkSharp.Client;

namespace LegoInventoryHelper.DatabaseContext.Entities
{
    public class Price
    {
        public int ID { get; set; }
        public string SetID { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public double AveragePrice { get; set; }
        public double QuantityAveragePrice { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;

        public Price() { }
        public Price(PriceGuide priceGuide, string setID)
        {
            SetID = setID;
            MinPrice = decimal.ToDouble(priceGuide.MinPrice);
            MaxPrice = decimal.ToDouble(priceGuide.MaxPrice);
            AveragePrice = decimal.ToDouble(priceGuide.AveragePrice);
            QuantityAveragePrice = decimal.ToDouble(priceGuide.QuantityAveragePrice);
        }
    }
}