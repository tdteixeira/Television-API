namespace Television_API.Models
{
    public class Actor
    {
        public int id { get; set; }
        public String name { get; set; }
        public int age { get; set; }

        public ICollection<TVShow> tvShows { get; set; }
    }
}
