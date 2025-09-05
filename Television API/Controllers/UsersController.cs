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


        [HttpPost()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            return Ok(users);
        }

        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register(UserRegistrationDto request)
        {
            var success = await _userRepository.RegisterUser(request);
            if (!success)
            {
                return BadRequest("Username is already in use.");
            }

            return Ok("User registered successfully.");
        }
    }
}
