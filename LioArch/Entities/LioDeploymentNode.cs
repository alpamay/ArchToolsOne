using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LioArch.Modeling
{
    public class LioDeploymentNode
    {
        [JsonProperty("_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("DisplayName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayName { get; set; }

        [JsonProperty("Technology", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Technology { get; set; }

        [JsonProperty("RelationString", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string RelationString { get; set; }

        [JsonProperty("Nodes", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<LioChildRelation> Nodes { get; set; }

        [JsonProperty("Containers", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<LioContainerChildRelation> Containers { get; set; }
    }

    public class LioChildRelation
    {
        [JsonProperty("_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("Type", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
    }
}