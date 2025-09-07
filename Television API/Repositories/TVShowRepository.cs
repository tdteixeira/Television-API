using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Television_API.Data;
using Television_API.Models;

namespace Television_API.Repositories
{
    public interface ITVShowRepository
    {
        Task<TVShow> GetTVShowAsync(int id);
        Task<bool> AddTVShowAsync(TVShow show);
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
                .Where(e => e.TvShowId == showId)
                .ToListAsync();
            return _mapper.Map<List<EpisodeDto>>(episodes);
        }

        public async Task<IEnumerable<ActorDto>> GetTVShowActorsAsync(int showId)
        {
            var actors = await _context.Actors
                .Where(a => a.TvShows.Any(s => s.Id == showId))
                .Include(a => a.TvShows)
                .ToListAsync();
            return _mapper.Map<List<ActorDto>>(actors);
        }

        public async Task<IEnumerable<TVShowDto>> SearchShowsAsync(TVShowDto dto)
        {
            var query = _context.TVShows.AsQueryable();

            if (dto.Id.HasValue)
                query = query.Where(s => s.Id == dto.Id.Value);

            if (!string.IsNullOrWhiteSpace(dto.Title))
                query = query.Where(s => s.Title.Contains(dto.Title));

            if (dto.Genres != null && dto.Genres.Any())
                query = query.Where(s => s.Genres.Any(g => dto.Genres.Contains(g)));

            if (dto.StartDate.HasValue)
                query = query.Where(s => s.StartDate == dto.StartDate.Value);

            if (dto.IsOngoing.HasValue)
                query = query.Where(s => s.IsOngoing == dto.IsOngoing.Value);

            var results = await query.ToListAsync();

            return _mapper.Map<List<TVShowDto>>(results);
        }

        public async Task<TVShow> GetTVShowAsync(int id)
        {
            var show = await _context.TVShows
                .Include(s => s.Actors)
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id);
            return show;
        }

        public async Task<bool> AddTVShowAsync(TVShow show)
        {
            _context.TVShows.Add(show);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
