namespace Television_API.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Birthday { get; set; }
        public string? Deathday { get; set; }
        public ICollection<TVShow> TvShows { get; set; }
    }

    public class ActorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Birthday { get; set; }
        public string? Deathday { get; set; }
    }
}
