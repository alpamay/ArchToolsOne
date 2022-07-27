using System.Collections.Generic;
using System.Globalization;
using LioArch.Modeling;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LioArch.Persistence
{
    public class LioDeploymentNodes
    {
        [JsonProperty("LioDeploymentNodes", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<LioDeploymentNode> ContainerList { get; set; }

        public static List<LioDeploymentNode> FromJson(string json) =>
            JsonConvert.DeserializeObject<List<LioDeploymentNode>>(json, Converter.Settings);

        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
                {
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                },
            };
        }
    }
}