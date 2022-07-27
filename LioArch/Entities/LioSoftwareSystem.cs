using System.Collections.Generic;
using LioArch.Modeling;
using Newtonsoft.Json;

//    EXAMPLE:
//
// using LioArch.Modeling;
// var lioSoftwareSystems = LioSoftwareSystems.FromJson(jsonString);

namespace LioArch.Entities
{
    public class LioSoftwareSystem
    {
        [JsonProperty("_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("Location", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Location { get; set; }

        [JsonProperty("DisplayName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayName { get; set; }

        [JsonProperty("Type", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("Technology", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Technology { get; set; }

        [JsonProperty("RelationString", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string RelationString { get; set; }

        [JsonProperty("RelatedWith", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<LioRelation> RelatedWith { get; set; }
    }
}


