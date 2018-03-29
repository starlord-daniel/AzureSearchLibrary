using System;
using Newtonsoft.Json;

namespace AzureSearch.Model
{
    [Serializable]
    public class AzureSearchResult
    {
        public string odatacontext { get; set; }
        public Value[] value { get; set; }
    }

    [Serializable]
    public class Value
    {
        [JsonProperty(PropertyName = "@search.score")]
        public float searchscore { get; set; }

        public int id { get; set; }

        public string question { get; set; }

        public string answer { get; set; }

        [JsonProperty(PropertyName = "follow_up")]
        public int followup { get; set; }
    }
}
