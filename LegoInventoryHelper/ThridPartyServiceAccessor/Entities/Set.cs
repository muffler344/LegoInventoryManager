using Newtonsoft.Json;

namespace ThridPartyServiceAccessor.Entities
{
    public class Set
    {
        [JsonProperty("set_num")]
        public string SetNumber { get; set; } = "";
        [JsonProperty("name")]
        public string Name { get; set; } = "";
        [JsonProperty("year")]
        public int YearOfRelease { get; set; }
        [JsonProperty("theme_id")]
        public int ThemeID { get; set; }
        [JsonProperty("num_parts")]
        public int NumberOfParts { get; set; }
        [JsonProperty("set_img_url")]
        public string SetImageURL { get; set; } = "";
        [JsonProperty("set_url")]
        public string RebrickableSetURL { get; set; } = "";
        [JsonProperty("last_modified_dt")]
        public DateTime LastModifiedDate { get; set; }
    }
}