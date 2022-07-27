using System.Collections.Generic;
using LioArch.Modeling;
using Newtonsoft.Json;

namespace LioArch.Entities
{
    public class LioPersona
    {
        [JsonProperty("_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("CanonicalName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string CanonicalName { get; set; }

        [JsonProperty("Description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        
        [JsonProperty("Uses", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<LioPersonaUsesRelation> Uses { get; set; }
    }
}