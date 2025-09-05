using Microsoft.AspNetCore.Mvc;
using System.Net;
using Television_API.Models;
using Television_API.Repositories;

namespace Television_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActorsController: ControllerBase
    {
        private readonly IActorRepository _repository;

        public ActorsController(IActorRepository repository)
        {
            _repository = repository;
        }

        [HttpGet(Name = "GetActors")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ActorDto>))]
        public async Task<IActionResult> GetActors()
        {
            var actors = await _repository.GetActors();
            return Ok(actors);
        }

    }
}
