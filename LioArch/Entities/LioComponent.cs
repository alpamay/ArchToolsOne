using System.Collections.Generic;
using System.ComponentModel;
using LioArch.Modeling;
using Newtonsoft.Json;

namespace LioArch.Entities
{
    public class LioComponent
    {
        [JsonProperty("_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("CanonicalName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string CanonicalName { get; set; }

        [JsonProperty("Description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("Technology", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Technology { get; set; }

        [JsonProperty("Uses", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<LioRelation> Uses { get; set; }

        [DefaultValue("")]
        [JsonProperty("Tag", DefaultValueHandling = DefaultValueHandling.Populate)]
        public string Tag { get; set; }
    }
}