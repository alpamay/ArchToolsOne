using Newtonsoft.Json;

namespace LioArch.Modeling
{
    public class LioPersonaUsesRelation
    {
        [JsonProperty("Target", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Target { get; set; }

        [JsonProperty("Action", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }
    }
}