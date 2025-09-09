using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Television_API.Data;
using Television_API.Models;
using Television_API.Models.ExternalAPIModels;
namespace Television_API.Services
{
    public class TVShowFetcherService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<TVShowFetcherService> _logger;

        private const string ApiMostPopularUrl = "https://www.episodate.com/api/most-popular?page={0}";
        private const string ApiShowDetailsUrl = "https://www.episodate.com/api/show-details?q={0}";
        private const string ApiSearchTVMazeUrl = "https://api.tvmaze.com/search/shows?q={0}";
        private const string ApiGetCastUrl = "https://api.tvmaze.com/shows/{0}/cast";

        public TVShowFetcherService(IServiceProvider services, ILogger<TVShowFetcherService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var httpClient = new HttpClient();

            int episodatepage = 1;
            int maxpages = 3; // Limit to first 3 pages for demo purposes
            while (!stoppingToken.IsCancellationRequested && episodatepage<=maxpages)
            {
                try
                {
                    string paged_url = string.Format(ApiMostPopularUrl, episodatepage);
                    var response = await httpClient.GetStringAsync(paged_url, stoppingToken);
                    var data = JsonSerializer.Deserialize<EpisodateResponse>(response);

                    _logger.LogInformation("Fetched page {page} from Episodate", episodatepage);
                    foreach (var show in data.tv_shows)
                    {
                        var existingShow = await db.TVShows.FirstOrDefaultAsync(s => s.Title == show.name, stoppingToken);
                        var detailedshow = await GetDetailedEpisodateTvShow(show.id, httpClient);

                        if (existingShow == null)
                        {
                            await AddNewShow(httpClient, db, detailedshow);
                        }
                    }
                    _logger.LogInformation("TV shows from page {page} synced at {time}", episodatepage, DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing TV shows");
                }
                episodatepage++;
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }

        }

        private async Task AddNewShow(HttpClient httpClient, AppDbContext db, EpisodateShowDetailed show)
        {
            var parsedDate = DateOnly.TryParse(show.startDate, out var date) ? date : DateOnly.MinValue;
            var isOngoing = show.status == "Running";
            ICollection<Episode> episodes = getShowEpisodes(show);
            ICollection<Actor> actors = await GetShowCastAsync(httpClient, show);

            // Add new show
            var newShow = new TVShow
            {
                Title = show.name,
                Genres = show.genres,
                StartDate = parsedDate,
                IsOngoing = isOngoing,
                Episodes = new List<Episode>(),
                Actors = new List<Actor>(),
                FavoritedByUsers = new List<User>()
            };
            db.TVShows.Add(newShow);
            await db.SaveChangesAsync();

            // Add episodes
            foreach (var ep in episodes)
            {
                ep.TvShowId = newShow.Id; // Set foreign key
                ep.TvShow = newShow; // Link via navigation
                newShow.Episodes.Add(ep);
            }

            //TODO Add actors
            foreach (var actor in actors)
            {
                var existingActor = await db.Actors.FirstOrDefaultAsync(a => a.Name == actor.Name);
                if (existingActor == null)
                {
                    existingActor = new Actor
                    {
                        Name = actor.Name,
                        Birthday = actor.Birthday,
                        Deathday = actor.Deathday,
                        TvShows = new List<TVShow>()
                    };
                    db.Actors.Add(existingActor);
                }
                existingActor.TvShows.Add(newShow);
                newShow.Actors.Add(existingActor);
            }
            await db.SaveChangesAsync();
        }

        private async Task<ICollection<Actor>> GetShowCastAsync(HttpClient httpClient, EpisodateShowDetailed show)
        {
            var actors = new List<Actor>();

            var search_url = string.Format(ApiSearchTVMazeUrl, Uri.EscapeDataString(show.name));
            var searchResponse = await httpClient.GetStringAsync(search_url);
            var searchData = JsonSerializer.Deserialize<List<TVMazeShowSearchResult>>(searchResponse);
            if (searchData == null || searchData.Count == 0)
            {
                _logger.LogWarning("No TVMaze show found for " + show.name);
                return actors;
            }

            var tvMazeShow = searchData.First().show;
            var cast_url = string.Format(ApiGetCastUrl, tvMazeShow.id);
            var castResponse = await httpClient.GetStringAsync(cast_url);
            var castData = JsonSerializer.Deserialize<List<TVMazeCastMember>>(castResponse);
            if (castData == null)
            {
                _logger.LogWarning("No cast data found for TVMaze show " + tvMazeShow.name);
                return actors;
            }

            foreach (var member in castData)
            {
                var person = member.person;
                var resultParsedBirthDay = DateOnly.TryParse(person.birthday, out var parsedBirthDay);
                var resultParsedDeathDay = DateOnly.TryParse(person.deathday, out var parsedDeathDay);
                actors.Add(new Actor
                {
                    Name = person.name,
                    Birthday = resultParsedBirthDay ? parsedBirthDay : null,
                    Deathday = resultParsedDeathDay ?  parsedDeathDay: null,
                });
            }
            return actors;
        }

        private ICollection<Episode> getShowEpisodes(EpisodateShowDetailed show)
        {
            var episodes = new List<Episode>();
            if (show.episodes != null)
            {
                foreach (var episode in show.episodes)
                {
                    var parsedDate = DateOnly.FromDateTime(DateTime.TryParse(episode.airDate, out var date) ? date : DateTime.MinValue);
                    episodes.Add(new Episode
                    {
                        Season = episode.season,
                        EpisodeNumber = episode.episodeNumber,
                        Name = episode.name,
                        AirDate = parsedDate,
                    });
                }
            }
            return episodes;
        }

        private static async Task<EpisodateShowDetailed> GetDetailedEpisodateTvShow(int episodateId, HttpClient httpClient)
        {
            string details_url = string.Format(ApiShowDetailsUrl, episodateId);
            var detailsResponse = await httpClient.GetStringAsync(details_url);
            var detailsData = JsonSerializer.Deserialize<EpisodateShowDetailedWrapper>(detailsResponse);
            if (detailsData == null || detailsData.tvShow == null)
            {
                throw new Exception("Failed to fetch detailed show data");
            }
            return detailsData.tvShow;
        }
    }
}
