using Microsoft.EntityFrameworkCore;
using System.Net.Http;
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
                try
                {
                    string paged_url = string.Format(ApiMostPopularUrl, 1);
                    var response = await httpClient.GetStringAsync(paged_url, stoppingToken);
                    var data = JsonSerializer.Deserialize<EpisodateResponse>(response);

                    foreach (var show in data.tv_shows)
                    {
                        var existingShow = await db.TVShows.FirstOrDefaultAsync(s => s.title == show.name, stoppingToken);

                        var parsedDate = DateOnly.TryParse(show.start_date, out var date) ? date : DateOnly.MinValue;
                        var genre = show.genre?.FirstOrDefault() ?? "Unknown";
                        var isOngoing = show.status == "Running";

                        if (existingShow == null)
                        {
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

                            foreach (var ep in episodes)
                            {
                                ep.tvShowId = newShow.id; // Set foreign key
                                ep.tvShow = newShow; // Link via navigation property
                                newShow.episodes.Add(ep);
                            }
                            await db.SaveChangesAsync();

                        }
                        else
                        {
                            // Update only if something changed
                            if (existingShow.genre != genre ||
                                existingShow.startDate != parsedDate ||
                                existingShow.isOngoing != isOngoing)
                            {
                                existingShow.genre = genre;
                                existingShow.startDate = parsedDate;
                                existingShow.isOngoing = isOngoing;
                            }
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("TV shows synced at {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing TV shows");
                }

                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }

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
