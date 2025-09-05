using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Television_API.Data;
using Television_API.Models;

namespace Television_API.Repositories
{
    public interface ITVShowRepository
    {
        Task<IEnumerable<TVShowDto>> GetTVShows();
        Task<IEnumerable<EpisodeDto>> GetTVShowEpisodes(int showId);
        Task<IEnumerable<ActorDto>> GetTVShowActors(int showId);
    }

    public class TVShowRepository : ITVShowRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public TVShowRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TVShowDto>> GetTVShows()
        {
            var tvShows = await _context.TVShows.ToListAsync();
            return _mapper.Map<List<TVShowDto>>(tvShows);
        }

        public async Task<IEnumerable<EpisodeDto>> GetTVShowEpisodes(int showId)
        {
            var episodes = await _context.Episodes
                .Where(e => e.tvShowId == showId)
                .ToListAsync();
            return _mapper.Map<List<EpisodeDto>>(episodes);
        }

        public async Task<IEnumerable<ActorDto>> GetTVShowActors(int showId)
        {
            var actors = await _context.Actors
                .Where(a => a.tvShows.Any(s => s.id == showId))
                .Include(a => a.tvShows)
                .ToListAsync();
            return _mapper.Map<List<ActorDto>>(actors);
        }
    }
}
