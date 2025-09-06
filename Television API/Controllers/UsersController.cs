using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Television_API.Models;
using Television_API.Repositories;

namespace Television_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController: ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("{username}/favorite-shows")]
        [ProducesResponseType((int) HttpStatusCode.OK, Type = typeof(IEnumerable<TVShowDto>))]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetFavoriteShows(string username)
        {
            var favorites = await _userRepository.GetFavoriteShowsAsync(username);
            if (favorites == null) 
            {
                return NotFound("User not found");
            }
            return Ok(favorites);
        }
        [HttpPost("add/favorite-shows")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddFavoriteShow(int showId)
        {
            var username = User.Identity?.Name;
            var hasAdded = await _userRepository.AddFavoriteShowAsync(username!, showId);
            if (hasAdded)
            {
                return Ok("Added favorited show");
            }
            else
            {
                return BadRequest("Invalid show or show was already favorited by user");
            }
        }
        [HttpDelete("delete/favorite-shows")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteFavoriteShow(int showId)
        {
            var username = User.Identity?.Name;
            var hasDeleted = await _userRepository.RemoveFavoriteShowAsync(username!, showId);
            if (hasDeleted)
            {
                return Ok("Favorited show removed");
            }
            else 
            {
                return BadRequest("Invalid show or show wasn't favorited by user");
            }
        }

    }
}
