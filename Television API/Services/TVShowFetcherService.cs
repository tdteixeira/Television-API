using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Television_API.Data;
using Television_API.Models;
namespace Television_API.Services {
    public class TVShowFetcherService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<TVShowFetcherService> _logger;

        private const string ApiMostPopularUrl = "https://www.episodate.com/api/most-popular?page={page}";
        private const string ApiShowDetailsUrl = "https://www.episodate.com/api/show-details?q={showId}";

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

                        if (existingShow == null)
                        {
                            await AddNewShow(httpClient,db,show);
                        }
                        else
                        { 
                            await EditExistingShow(httpClient, db, existingShow, show);
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

        private async Task EditExistingShow(HttpClient httpClient, AppDbContext db, TVShow existingShow, EpisodateShow show)
        {
            var parsedDate = DateOnly.TryParse(show.start_date, out var date) ? date : DateOnly.MinValue;
            var genre = show.genre?.FirstOrDefault() ?? "Unknown";
            var isOngoing = show.status == "Running";
            _logger.LogInformation("Editing show: " + show.name);
            ICollection<Episode> episodes = await getShowEpisodesAsync(show.id, httpClient);
            _logger.LogInformation("Fetched episodes(up): " + episodes.Count + " for show " + show.name);
            // Update existing show
            existingShow.title = show.name;
            existingShow.genre = genre;
            existingShow.startDate = parsedDate;
            existingShow.isOngoing = isOngoing;
            await db.SaveChangesAsync();
            //TODO Update episodes
            //TODO Update actors

        }

        private async Task AddNewShow(HttpClient httpClient,AppDbContext db, EpisodateShow show)
        {
            var parsedDate = DateOnly.TryParse(show.start_date, out var date) ? date : DateOnly.MinValue;
            var genre = show.genre?.FirstOrDefault() ?? "Unknown";
            var isOngoing = show.status == "Running";
            _logger.LogInformation("Adding new show: " + show.name);
            ICollection<Episode> episodes = await getShowEpisodesAsync(show.id, httpClient);
            _logger.LogInformation("Fetched episodes(up): " + episodes.Count + " for show " + show.name);
            // Add new show
            var newShow = new TVShow
            {
                title = show.name,
                genre = genre,
                startDate = parsedDate,
                isOngoing = isOngoing,
                episodes = new List<Episode>()
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

            //Add actors
        }

        private async Task<ICollection<Episode>> getShowEpisodesAsync(int id, HttpClient httpClient)
        {
            var episodes = new List<Episode>();
            string details_url = string.Format(ApiShowDetailsUrl, id);
            var detailsResponse = await httpClient.GetStringAsync(details_url);
            var detailsData = JsonSerializer.Deserialize<EpisodateShowDetailedWrapper>(detailsResponse).tvShow;
            if (detailsData == null) {
                _logger.LogInformation("\nDETAILS DATA FAILED TO DESERIALIZE(again)\n");
            }
            if (detailsData?.episodes != null)
            {
                foreach (var episode in detailsData.episodes)
                {
                    _logger.LogInformation("Processing episode: " + episode.name + " " + episode.airDate);
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
            return episodes;
        }
    }
}
