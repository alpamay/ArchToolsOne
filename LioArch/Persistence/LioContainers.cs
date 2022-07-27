using System.Collections.Generic;
using System.Globalization;
using LioArch.Entities;
using LioArch.Modeling;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LioArch.Persistence
{
    public class LioContainers
    {
        [JsonProperty("LioContainers", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<LioContainer> ContainerList { get; set; }

        public static List<LioContainer> FromJson(string json) => 
            JsonConvert.DeserializeObject<List<LioContainer>>(json, Converter.Settings);

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