namespace Television_API.Models
{
    public class EpisodateResponse
    {
        public List<EpisodateShow> tv_shows { get; set; }
    }

    public class EpisodateShow
    {
        public string name { get; set; }
        public List<string> genre { get; set; }
        public string start_date { get; set; }
        public string status { get; set; }
    }

}
