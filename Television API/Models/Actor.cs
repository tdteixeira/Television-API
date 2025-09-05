namespace Television_API.Models
{
    public class Actor
    {
        public int id { get; set; }
        public String name { get; set; }
        public string? birthday { get; set; }
        public string? deathday { get; set; }
        public ICollection<TVShow> tvShows { get; set; }
    }

    public class ActorDto
    {
        public String name { get; set; }
        public string birthday { get; set; }
        public string deathday { get; set; }
    }
}
