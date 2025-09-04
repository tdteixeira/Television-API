namespace Television_API.Models
{
    public class Episode
    {
        public int id { get; set; }
        public int season { get; set; }
        public int episode { get; set; }
        public string name { get; set; }
        public string air_date { get; set; }
        public int tvShowId { get; set; }
        public TVShow tvShow { get; set; }
    }
}
