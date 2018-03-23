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

        public int followup { get; set; }
    }
}
