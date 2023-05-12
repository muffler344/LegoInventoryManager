using Newtonsoft.Json;

namespace ThridPartyServiceAccessor.Entities
{
    public class ThemeRebrickable
    {
        [JsonProperty("id")]
        public int ThemeID { get; set; }
        [JsonProperty("parent_id")]
        public int? ParentID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; } = "";
    }
}
