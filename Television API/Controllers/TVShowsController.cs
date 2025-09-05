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

        [HttpGet(Name = "GetTVShows")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<TVShowDto>))]
        public async Task<IActionResult> GetTVShows()
        {
            var tvshows = await _repository.GetTVShowsAsync();
            return Ok(tvshows);
        }

        [HttpGet("{showId:int}/episodes",Name = "GetTVShowEpisodes")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<EpisodeDto>))]
        public async Task<IActionResult> GetTVShowEpisodes(int showId)
        {
            var episodes = await _repository.GetTVShowEpisodesAsync(showId);
            return Ok(episodes);
        }

        [HttpGet("{showId:int}/actors",Name = "GetTVShowActors")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ActorDto>))]
        public async Task<IActionResult> GetTVShowActors(int showId)
        {
            var actors = await _repository.GetTVShowActorsAsync(showId);
            return Ok(actors);
        }

        [HttpPost("search", Name = "SearchTVShows")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<TVShowDto>))]
        public async Task<IActionResult> SearchTVShows([FromQuery] TVShowDto dto)
        {
            var results = await _repository.SearchShowsAsync(dto);
            return Ok(results);
        }
    }

}
