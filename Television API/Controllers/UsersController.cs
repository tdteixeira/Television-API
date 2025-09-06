using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
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
            var users = await _userRepository.GetUsers();
            return Ok(users);
        }

        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register(UserRequestDto request)
        {
            var success = await _userRepository.RegisterUser(request);
            if (!success)
            {
                return BadRequest("Username is already in use.");
            }

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Login(UserRequestDto request)
        {
            var token = await _userRepository.LoginUser(request);
            if (token == null) 
            {
                return Unauthorized("Invalid Password or Username");
            }

            return Ok(token);
        }

        [HttpGet("{username}/favoriteShows")]
        public async Task<IActionResult> GetFavoriteShows(string username)
        {
            var favorites = await _userRepository.GetFavoriteShows(username);
            if (favorites == null) 
            {
                return NotFound("User not found");
            }
            return Ok(favorites);
        }
        [HttpPost("add/favoriteShows")]
        [Authorize]
        public async Task<IActionResult> AddFavoriteShow(int showId)
        {
            var username = User.Identity?.Name;
            var hasAdded = await _userRepository.AddFavoriteShow(username!, showId);
            if (hasAdded)
            {
                return Ok("Added favorited show");
            }
            else
            {
                return BadRequest("Invalid show or show was already favorited by user");
            }
        }
        [HttpDelete("delete/favoriteShows")]
        [Authorize]
        public async Task<IActionResult> DeleteFavoriteShow(int showId)
        {
            var username = User.Identity?.Name;
            var hasDeleted = await _userRepository.RemoveFavoriteShow(username!, showId);
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
