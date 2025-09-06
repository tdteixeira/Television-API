using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Television_API.Models;
using Television_API.Repositories;

namespace Television_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FavoriteShowsController : ControllerBase
    {
        [HttpGet("secure")]
        [Authorize]
        public IActionResult GetSecureData()
        {
            var username = User.Identity?.Name;
            return Ok(new
            {
                message = "You are authorized!",
                user = username
            });
        }
    }
}
