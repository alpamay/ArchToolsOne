using Newtonsoft.Json;

namespace LioArch.Modeling
{
    public class LioContainerChildRelation
    {
        [JsonProperty("CanonicalName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string CanonicalName { get; set; }

        [JsonProperty("LifetimeResp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string LifetimeResp { get; set; }
    }
}