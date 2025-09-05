using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Television_API.Data;
using Television_API.Models;

namespace Television_API.Repositories
{
    public interface ITVShowRepository
    {
        Task<IEnumerable<TVShowDto>> GetTVShowsAsync();
        Task<IEnumerable<EpisodeDto>> GetTVShowEpisodesAsync(int showId);
        Task<IEnumerable<ActorDto>> GetTVShowActorsAsync(int showId);
        Task<IEnumerable<TVShowDto>> SearchShowsAsync(TVShowDto dto);
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

        public async Task<IEnumerable<TVShowDto>> GetTVShowsAsync()
        {
            var tvShows = await _context.TVShows.ToListAsync();
            return _mapper.Map<List<TVShowDto>>(tvShows);
        }

        public async Task<IEnumerable<EpisodeDto>> GetTVShowEpisodesAsync(int showId)
        {
            var episodes = await _context.Episodes
                .Where(e => e.tvShowId == showId)
                .ToListAsync();
            return _mapper.Map<List<EpisodeDto>>(episodes);
        }

        public async Task<IEnumerable<ActorDto>> GetTVShowActorsAsync(int showId)
        {
            var actors = await _context.Actors
                .Where(a => a.tvShows.Any(s => s.id == showId))
                .Include(a => a.tvShows)
                .ToListAsync();
            return _mapper.Map<List<ActorDto>>(actors);
        }

        public async Task<IEnumerable<TVShowDto>> SearchShowsAsync(TVShowDto dto)
        {
            var query = _context.TVShows.AsQueryable();

            if (dto.id.HasValue)
                query = query.Where(s => s.id == dto.id.Value);

            if (!string.IsNullOrWhiteSpace(dto.title))
                query = query.Where(s => s.title.Contains(dto.title));

            if (dto.genres != null && dto.genres.Any())
                query = query.Where(s => s.genres.Any(g => dto.genres.Contains(g)));

            if (dto.startDate.HasValue)
                query = query.Where(s => s.startDate == dto.startDate.Value);

            if (dto.isOngoing.HasValue)
                query = query.Where(s => s.isOngoing == dto.isOngoing.Value);

            var results = await query.ToListAsync();

            return _mapper.Map<List<TVShowDto>>(results);
        }

    }
}
