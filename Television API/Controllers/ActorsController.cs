using Microsoft.AspNetCore.Mvc;
using System.Net;
using Television_API.Models;
using Television_API.Repositories;

namespace Television_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActorsController : ControllerBase
    {
        private readonly IActorRepository _repository;

        public ActorsController(IActorRepository repository)
        {
            _repository = repository;
        }

        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ActorDto>))]
        public async Task<IActionResult> GetActors([FromQuery] PaginationParams p)
        {
            var actors = await _repository.GetPagedActorsAsync(p);
            return Ok(actors);
        }

        [HttpGet("{actorId:int}/tvshows")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<TVShowDto>))]
        public async Task<IActionResult> GetActorTVShow(int actorId)
        {
            var actor = await _repository.GetActorAsync(actorId);
            if (actor == null)
            {
                return NotFound();
            }
            var shows = await _repository.GetTVShowFromActorAsync(actorId);
            return Ok(shows);
        }

    }
}
