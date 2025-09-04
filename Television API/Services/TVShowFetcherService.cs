using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Television_API.Data;
using Television_API.Models;
namespace Television_API.Services {
    public class TVShowFetcherService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<TVShowFetcherService> _logger;

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
                    var response = await httpClient.GetStringAsync("https://www.episodate.com/api/most-popular?page=1", stoppingToken);
                    var data = JsonSerializer.Deserialize<EpisodateResponse>(response);

                    foreach (var show in data.tv_shows)
                    {
                        var existingShow = await db.TVShows.FirstOrDefaultAsync(s => s.title == show.name, stoppingToken);

                        var parsedDate = DateOnly.FromDateTime(DateTime.TryParse(show.start_date, out var date) ? date : DateTime.MinValue);
                        var genre = show.genre?.FirstOrDefault() ?? "Unknown";
                        var isOngoing = show.status == "Running";

                        if (existingShow == null)
                        {
                            // Add new show
                            db.TVShows.Add(new TVShow
                            {
                                title = show.name,
                                genre = genre,
                                startDate = parsedDate,
                                isOngoing = isOngoing
                            });
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
    }
}
