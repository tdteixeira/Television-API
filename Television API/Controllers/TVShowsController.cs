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
        public async Task<IEnumerable<TVShowDto>> GetTVShows()
        {
            var tvShows = await _context.TVShows
                .ToListAsync();

            return _mapper.Map<List<TVShowDto>>(tvShows);
        }

        [HttpGet("{showId:int}/episodes",Name = "GetTVShowEpisodes")]
        public async Task<IEnumerable<EpisodeDto>> GetTVShowEpisodes(int showId)
        {
            var episodes = await _context.Episodes
                .Where(e => e.tvShowId == showId)
                .ToListAsync();

            return _mapper.Map<List<EpisodeDto>>(episodes);

        }

        [HttpGet("{showId:int}/actors",Name = "GetTVShowActors")]
        public async Task<IEnumerable<ActorDto>> GetTVShowActors(int showId)
        {
            var tvShows = await _context.Actors
                .Where(a => a.tvShows.Any(s => s.id == showId))
                .Include(a => a.tvShows)
                .ToListAsync();

            return _mapper.Map<List<ActorDto>>(tvShows);
        }

    }

}
