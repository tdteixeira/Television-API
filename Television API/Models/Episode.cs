namespace Television_API.Models
{
    public class Episode
    {
        public int Id { get; set; }
        public int Season { get; set; }
        public int EpisodeNumber { get; set; }
        public string Name { get; set; }
        public DateOnly AirDate { get; set; }
        public int TvShowId { get; set; }
        public TVShow TvShow { get; set; }
    }

    public class EpisodeDto
    {
        public int Season { get; set; }
        public int EpisodeNumber { get; set; }
        public string Name { get; set; }
        public DateOnly AirDate { get; set; }
    }
}
