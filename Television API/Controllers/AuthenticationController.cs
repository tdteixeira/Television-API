using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Television_API.Models;
using Television_API.Repositories;

namespace Television_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public AuthenticationController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register(UserRequestDto request)
        {
            var success = await _userRepository.RegisterUserAsync(request);
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
            var token = await _userRepository.LoginUserAsync(request);
            if (token == null)
            {
                return Unauthorized("Invalid Password or Username");
            }

            return Ok(token);
        }
    }
}
