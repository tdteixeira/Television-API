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

        /// <summary>
        /// Retrieves a paginated list of all registered users.
        /// </summary>
        /// <param name="p">Pagination parameters - page number and page size.</param>
        /// <returns>A list of users wrapped in an HTTP 200 OK response.</returns>
        [HttpGet]
        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUsers([FromQuery]PaginationParams p)
        {
            var users = await _userRepository.GetPagedUsersAsync(p);
            return Ok(users);
        }

        /// <summary>
        /// Retrieves a paginated list of favorite TV shows for a specific user.
        /// </summary>
        /// <param name="p">Pagination parameters - page number and page size.</param>
        /// <param name="username">The username of the user whose favorites are being retrieved.</param>
        /// <returns>
        /// A list of <see cref="TVShowDto"/> objects if the user exists; otherwise, a 404 Not Found response.
        /// </returns>
        [HttpGet("{username}/favorite-shows")]
        [ProducesResponseType((int) HttpStatusCode.OK, Type = typeof(IEnumerable<TVShowDto>))]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetFavoriteShows([FromQuery]PaginationParams p, string username)
        {
            var favorites = await _userRepository.GetPagedFavoriteShowsAsync(p,username);
            if (favorites == null) 
            {
                return NotFound("User not found");
            }
            return Ok(favorites);
        }

        /// <summary>
        /// Adds a TV show to the authenticated user's list of favorites.
        /// </summary>
        /// <param name="showId">The ID of the TV show to add.</param>
        /// <returns>
        /// HTTP 200 OK if the show was added successfully; otherwise, HTTP 400 Bad Request.
        /// </returns>
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
        /// <summary>
        /// Removes a TV show from the authenticated user's list of favorites.
        /// </summary>
        /// <param name="showId">The ID of the TV show to remove.</param>
        /// <returns>
        /// HTTP 200 OK if the show was removed successfully; otherwise, HTTP 400 Bad Request.
        /// </returns>
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
