using Microsoft.AspNetCore.Mvc;
using System.Net;
using Television_API.Models;
using Television_API.Repositories;


namespace Television_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TVShowsController : ControllerBase
    {
        private readonly ITVShowRepository _repository;

        public TVShowsController(ITVShowRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retrieves a paginated list of all TV shows.
        /// </summary>
        /// <param name="p">Pagination parameters - page number and page size.</param>
        /// <returns>A list of <see cref="TVShowDto"/> objects.</returns>
        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<TVShowDto>))]
        public async Task<IActionResult> GetTVShows([FromQuery] PaginationParams p)
        {
            var tvshows = await _repository.GetPagedTVShowsAsync(p);
            return Ok(tvshows);
        }

        /// <summary>
        /// Retrieves all episodes for a specific TV show.
        /// </summary>
        /// <param name="showId">The ID of the TV show.</param>
        /// <returns>A list of <see cref="EpisodeDto"/> objects.</returns>
        [HttpGet("{showId:int}/episodes")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<EpisodeDto>))]
        public async Task<IActionResult> GetTVShowEpisodes(int showId)
        {
            var episodes = await _repository.GetTVShowEpisodesAsync(showId);
            return Ok(episodes);
        }

        /// <summary>
        /// Retrieves all actors associated with a specific TV show.
        /// </summary>
        /// <param name="showId">The ID of the TV show.</param>
        /// <returns>A list of <see cref="ActorDto"/> objects.</returns>
        [HttpGet("{showId:int}/actors")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ActorDto>))]
        public async Task<IActionResult> GetTVShowActors(int showId)
        {
            var actors = await _repository.GetTVShowActorsAsync(showId);
            return Ok(actors);
        }

        /// <summary>
        /// Searches TV shows using filter criteria and returns a paginated result.
        /// </summary>
        /// <param name="p">Pagination parameters - page number and page size.</param>
        /// <param name="dto">Filter criteria - title, genres, status, startDate, id.</param>
        /// <returns>A filtered list of <see cref="TVShowDto"/> objects.</returns>
        [HttpPost("search")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<TVShowDto>))]
        public async Task<IActionResult> SearchTVShows([FromQuery] PaginationParams p, [FromQuery] TVShowDto dto)
        {
            var results = await _repository.PagedSearchShowsAsync(p,dto);
            return Ok(results);
        }
    }

}
