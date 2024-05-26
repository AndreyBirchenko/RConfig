using System;

using Unity.Plastic.Newtonsoft.Json;

namespace RConfig.Runtime
{
    [Serializable]
    public class SchemeData
    {
        [JsonProperty("scheme_name")] public string SchemeName;
        [JsonProperty("updated_at")] public DateTime UpdatedAt;
        [JsonProperty("csv")] public string Csv;
    }
}