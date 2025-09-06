using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Television_API.Data;
using Television_API.Models;
namespace Television_API.Services {
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

            while (!stoppingToken.IsCancellationRequested)
            {
                int episodatepage = 1;
                try
                {
                    string paged_url = string.Format(ApiMostPopularUrl,episodatepage);
                    var response = await httpClient.GetStringAsync(paged_url, stoppingToken);
                    var data = JsonSerializer.Deserialize<EpisodateResponse>(response);

                    foreach (var show in data.tv_shows)
                    {
                        var existingShow = await db.TVShows.FirstOrDefaultAsync(s => s.title == show.name, stoppingToken);
                        var detailedshow = await GetDetailedEpisodateTvShow(show.id, httpClient);

                        if (existingShow == null)
                        {
                            await AddNewShow(httpClient,db,detailedshow);
                        }
                        else
                        { 
                            await EditExistingShow(httpClient, db, existingShow, detailedshow);
                        }

                    }
                    _logger.LogInformation("TV shows from page {page} synced at {time}",episodatepage,DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing TV shows");
                }
                
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }

        }

        private async Task EditExistingShow(HttpClient httpClient, AppDbContext db, TVShow existingShow, EpisodateShowDetailed show)
        {
            _logger.LogInformation("Editing show: " + show.name);

            var parsedDate = DateOnly.TryParse(show.startDate, out var date) ? date : DateOnly.MinValue;
            var isOngoing = show.status == "Running";
            ICollection<Episode> episodes = getShowEpisodes(show);

            // Update existing show
            existingShow.title = show.name;
            existingShow.genres = show.genres;
            existingShow.startDate = parsedDate;
            existingShow.isOngoing = isOngoing;
            await db.SaveChangesAsync();
            //TODO Update episodes
            //TODO Update
            //TODO Maintain favorited users

        }

        private async Task AddNewShow(HttpClient httpClient,AppDbContext db, EpisodateShowDetailed show)
        {
            _logger.LogInformation("Adding new show: " + show.name);

            var parsedDate = DateOnly.TryParse(show.startDate, out var date) ? date : DateOnly.MinValue;
            var isOngoing = show.status == "Running";
            ICollection<Episode> episodes = getShowEpisodes(show);
            ICollection<Actor> actors = await GetShowCastAsync(httpClient, show);

            // Add new show
            var newShow = new TVShow
            {
                title = show.name,
                genres = show.genres,
                startDate = parsedDate,
                isOngoing = isOngoing,
                episodes = new List<Episode>(),
                actors = new List<Actor>(),
                favoritedByUsers = new List<User>()
            };
            db.TVShows.Add(newShow);
            await db.SaveChangesAsync();

            // Add episodes
            foreach (var ep in episodes)
            {
                ep.tvShowId = newShow.id; // Set foreign key
                ep.tvShow = newShow; // Link via navigation
                newShow.episodes.Add(ep);
            }
            await db.SaveChangesAsync();

            //TODO Add actors
            foreach (var actor in actors)
            {
                var existingActor = await db.Actors.FirstOrDefaultAsync(a => a.name == actor.name);
                if (existingActor == null)
                {
                    existingActor = new Actor
                    {
                        name = actor.name,
                        birthday = actor.birthday,
                        deathday = actor.deathday,
                        tvShows = new List<TVShow>()
                    };
                    db.Actors.Add(existingActor);
                    await db.SaveChangesAsync();
                }
                existingActor.tvShows.Add(newShow);
                newShow.actors.Add(existingActor);
            }
        }

        private async Task<ICollection<Actor>> GetShowCastAsync(HttpClient httpClient, EpisodateShowDetailed show)
        {
            var actors = new List<Actor>();

            var search_url = string.Format(ApiSearchTVMazeUrl, Uri.EscapeDataString(show.name));
            var searchResponse = await httpClient.GetStringAsync(search_url);
            var searchData = JsonSerializer.Deserialize<List<TVMazeShowSearchResult>>(searchResponse);
            _logger.LogInformation("IS SEARCHDATA EMPY: "+ searchData.Count);
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
                _logger.LogInformation("Found actor: " + member.person.name + " for show " + show.name);
                _logger.LogInformation("Birthday" + member.person.birthday + " Deathday " + member.person.deathday);
                var person = member.person;
                actors.Add(new Actor
                {
                    name = person.name,
                    birthday = person.birthday,
                    deathday = person.deathday,
                });
            }

            _logger.LogInformation("Fetched actors(up): " + actors.Count + " for show " + show.name);
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
                        season = episode.season,
                        episode = episode.episodeNumber,
                        name = episode.name,
                        airDate = parsedDate,
                    });
                }
            }
            _logger.LogInformation("Fetched episodes(up): " + episodes.Count + " for show " + show.name);
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
