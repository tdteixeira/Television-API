using System.Text.Json.Serialization;

namespace Television_API.Models.ExternalAPIModels
{
    public class EpisodateResponse
    {
        public ICollection<EpisodateShow> tv_shows { get; set; }
    }

    public class EpisodateShow
    {
        public int id { get; set; }
        public string name { get; set; }
        public string start_date { get; set; }
        public string status { get; set; }
    }

    public class EpisodateShowDetailedWrapper
    {
        [JsonPropertyName("tvShow")]
        public EpisodateShowDetailed tvShow { get; set; }
    }

    public class EpisodateShowDetailed
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("permalink")]
        public string permalink { get; set; }

        [JsonPropertyName("url")]
        public string url { get; set; }

        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("description_source")]
        public string descriptionSource { get; set; }

        [JsonPropertyName("start_date")]
        public string startDate { get; set; }

        [JsonPropertyName("end_date")]
        public string endDate { get; set; }

        [JsonPropertyName("country")]
        public string country { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }

        [JsonPropertyName("runtime")]
        public int runtime { get; set; }

        [JsonPropertyName("network")]
        public string network { get; set; }

        [JsonPropertyName("youtube_link")]
        public string youtubeLink { get; set; }

        [JsonPropertyName("image_path")]
        public string imagePath { get; set; }

        [JsonPropertyName("image_thumbnail_path")]
        public string imageThumbnailPath { get; set; }

        [JsonPropertyName("rating")]
        public string rating { get; set; }

        [JsonPropertyName("rating_count")]
        public int ratingCount { get; set; }

        [JsonPropertyName("countdown")]
        public object countdown { get; set; }

        [JsonPropertyName("genres")]
        public List<string> genres { get; set; }

        [JsonPropertyName("pictures")]
        public List<string> pictures { get; set; }

        [JsonPropertyName("episodes")]
        public List<EpisodateEpisode> episodes { get; set; }
    }

    public class EpisodateEpisode
    {
        [JsonPropertyName("season")]
        public int season { get; set; }

        [JsonPropertyName("episode")]
        public int episodeNumber { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("air_date")]
        public string airDate { get; set; }
    }
}
