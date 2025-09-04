using AutoMapper;
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
        private readonly IMapper _mapper;

        public TVShowsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetTVShows")]
        public async Task<IEnumerable<TVShowDto>> Get()
        {
            var tvShows = await _context.TVShows
                .Include(s => s.episodes)
                .ToListAsync();

            return _mapper.Map<List<TVShowDto>>(tvShows);
        }

    }

}
