namespace Television_API.Models
{
    public class TVShow
    {
        public int id { get; set; }
        public String title { get; set; }
        public String genre { get; set; }
        public DateOnly startDate { get; set; }
        public bool isOngoing { get; set; }
        public ICollection<Actor> actors { get; set; }
        public ICollection<Episode> episodes { get; set; }
    }

    public class TVShowDto
    {
        public String title { get; set; }
        public String genre { get; set; }
        public DateOnly startDate { get; set; }
        public bool isOngoing { get; set; }
        public ICollection<ActorDto> actors { get; set; }
        public ICollection<EpisodeDto> episodes { get; set; }
    }
}
