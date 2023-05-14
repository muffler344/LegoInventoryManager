using ThridPartyServiceAccessor.Entities;

namespace LegoInventoryHelper.DatabaseContext.Entities
{
    public class Theme
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ThemeID { get; set; }
        public int? ParentID { get; set; }
        public Theme() { }
        public Theme(ThemeRebrickable themeRebrickable)
        {
            Name = themeRebrickable.Name;
            ThemeID = themeRebrickable.ThemeID;
            ParentID = themeRebrickable.ParentID;
        }
    }
}