namespace LegoInventoryHelper.Models
{
    public class CreateInventoryItem
    {
        public string SetID { get; set; } = "";
        public double PriceBought { get; set; }
        public bool IsSellable { get; set; }
        public CreateInventoryItem(string setID, double priceBought, bool isSellable = true)
        {
            SetID = setID;
            PriceBought = priceBought;
            IsSellable = isSellable;
        }
    }
}
