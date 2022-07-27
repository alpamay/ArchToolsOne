using System.Collections.Generic;
using System.ComponentModel;
using LioArch.Modeling;
using Newtonsoft.Json;

namespace LioArch.Entities
{
    public class LioContainer
    {
        [JsonProperty("_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("CanonicalName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string CanonicalName { get; set; }

        [JsonProperty("Description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("Technology", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Technology { get; set; }

        [JsonProperty("Provides", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<LioInterface> Provides { get; set; }

        [JsonProperty("Uses", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<LioRelation> Uses { get; set; }

        [JsonProperty("Components", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Components { get; set; }

        [DefaultValue("")]
        [JsonProperty("Tag", DefaultValueHandling = DefaultValueHandling.Populate)]
        public string Tag { get; set; }

        public string ParentSoftwareSystemName { get; set; }

    }
}
