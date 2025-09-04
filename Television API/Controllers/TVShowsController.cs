using Microsoft.AspNetCore.Mvc;
using Television_API.Models;

namespace Television_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TVShowsController : ControllerBase
    {

        private readonly ILogger<TVShowsController> _logger;

        public TVShowsController(ILogger<TVShowsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetTVShows")]
        public IEnumerable<TVShow> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new TVShow
            {
                Id = index,
                Title = $"TV Show {index}",
                Genre = $"Genre {index}",
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                IsOngoing = index % 2 == 0
            })
            .ToArray();
        }
    }
}
