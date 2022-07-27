using Newtonsoft.Json;

namespace LioArch.Modeling
{
    public class LioRelation
    {
        [JsonProperty("Target", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Target { get; set; }

        [JsonProperty("Name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
}