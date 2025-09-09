namespace Television_API.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly? Birthday { get; set; }
        public DateOnly? Deathday { get; set; }
        public ICollection<TVShow> TvShows { get; set; } = new List<TVShow>();
    }

    public class ActorDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public DateOnly? Birthday { get; set; }
        public DateOnly? Deathday { get; set; }
    }
}
