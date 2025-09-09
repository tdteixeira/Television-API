namespace Television_API.Models
{
    public class TVShow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<string> Genres { get; set; } = new List<string>();
        public DateOnly StartDate { get; set; }
        public bool IsOngoing { get; set; }
        public ICollection<Actor> Actors { get; set; } = new List<Actor>();
        public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
        public ICollection<User> FavoritedByUsers { get; set; } = new List<User>();
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
