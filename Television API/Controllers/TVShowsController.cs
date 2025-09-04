using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Television_API.Data;
using Television_API.Models;


namespace Television_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TVShowsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TVShowsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetTVShows")]
        public async Task<IEnumerable<TVShow>> Get()
        {
            return await _context.TVShows.ToListAsync();
        }
    }

}
