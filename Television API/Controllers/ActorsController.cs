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

        /// <summary>
        /// Retrieves a paginated list of actors.
        /// </summary>
        /// <param name="p">Pagination parameters including page number and page size.</param>
        /// <returns>
        /// A list of <see cref="ActorDto"/> objects wrapped in an HTTP 200 OK response.
        /// </returns>
        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ActorDto>))]
        public async Task<IActionResult> GetActors([FromQuery] PaginationParams p)
        {
            var actors = await _repository.GetPagedActorsAsync(p);
            return Ok(actors);
        }

        /// <summary>
        /// Retrieves a list of all tvshows made by given actor.
        /// </summary>
        /// <param name="actorId">Id of the actor searched.</param>
        /// <returns>
        /// A list of <see cref="TVShowDto"/> objects wrapped in an HTTP 200 OK response. Or a 404 in case the given ID don't exist
        /// </returns>
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
