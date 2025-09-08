using Microsoft.AspNetCore.Mvc;
using System.Net;
using Television_API.Models;
using Television_API.Services;

namespace Television_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        public AuthenticationController(IAuthenticationService userRepository)
        {
            _authService = userRepository;
        }

        /// <summary>
        /// Registers a new user with the provided credentials.
        /// </summary>
        /// <param name="request">User registration details - username and password.</param>
        /// <returns>
        /// Returns HTTP 200 OK if registration is successful, or HTTP 400 Bad Request if the username is already in use.
        /// </returns>
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register(UserRequestDto request)
        {
            var success = await _authService.RegisterUserAsync(request);
            if (!success)
            {
                return BadRequest("Username is already in use.");
            }

            return Ok("User registered successfully.");
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token if credentials are valid.
        /// </summary>
        /// <param name="request">User login details - username and password.</param>
        /// <returns>
        /// Returns HTTP 200 OK with a JWT token if login is successful, or HTTP 401 Unauthorized if credentials are invalid.
        /// </returns>
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Login(UserRequestDto request)
        {
            var token = await _authService.LoginUserAsync(request);
            if (token == null)
            {
                return Unauthorized("Invalid Password or Username");
            }

            return Ok(token);
        }
    }
}
