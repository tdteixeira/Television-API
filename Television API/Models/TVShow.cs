namespace Television_API.Models
{
    public class TVShow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<string> Genres { get; set; }
        public DateOnly StartDate { get; set; }
        public bool IsOngoing { get; set; }
        public ICollection<Actor> Actors { get; set; }
        public ICollection<Episode> Episodes { get; set; }
        public ICollection<User> FavoritedByUsers { get; set; }
    }

    public class TVShowDto
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public ICollection<string>? Genres { get; set; }
        public DateOnly? StartDate { get; set; }
        public bool? IsOngoing { get; set; }

        public int NumberOfEpisodes { get; set; }
        public int NumberOfFavorites { get; set; }

    }
}
