
using System.Text.Json.Serialization;

namespace Television_API.Models.ExternalAPIModels
{
    public class TVMazeShowSearchResult
    {
        [JsonPropertyName("score")]
        public double Score { get; set; }
        [JsonPropertyName("show")]
        public TVMazeShow show { get; set; }
    }

    public class TVMazeShow
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
    }

    public class TVMazeCastMember
    {
        [JsonPropertyName("person")]
        public TVMazePerson person { get; set; }
    }

    public class TVMazePerson
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("birthday")]
        public string birthday { get; set; }
        [JsonPropertyName("deathday")]
        public string deathday { get; set; }
    }
}
