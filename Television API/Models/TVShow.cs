namespace Television_API.Models
{
    public class TVShow
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public String Genre { get; set; }
        public DateOnly StartDate { get; set; }
        public bool IsOngoing { get; set; }
    }
}
