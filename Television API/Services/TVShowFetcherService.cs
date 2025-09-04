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
                    var response = await httpClient.GetStringAsync("https://www.episodate.com/api/most-popular?page=1");
                    var data = JsonSerializer.Deserialize<EpisodateResponse>(response);

                    foreach (var show in data.tv_shows)
                    {
                        if (!db.TVShows.Any(s => s.Title == show.name))
                        {
                            db.TVShows.Add(new TVShow
                            {
                                Title = show.name,
                                Genre = show.genre?.FirstOrDefault() ?? "Unknown",
                                StartDate = DateOnly.FromDateTime(DateTime.TryParse(show.start_date, out var date) ? date : DateTime.Now),
                                IsOngoing = show.status == "Running"
                            });
                        }
                    }

                    await db.SaveChangesAsync();
                    _logger.LogInformation("TV shows updated at {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching TV shows");
                }

                await Task.Delay(TimeSpan.FromHours(6), stoppingToken); // Run every 6 hours
            }
        }
    }
}
