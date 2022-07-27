using Newtonsoft.Json;

namespace LioArch.Modeling
{
    public class LioInterface
    {
        [JsonProperty("Name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("Api", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Api { get; set; }
    }
}