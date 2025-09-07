using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Television_API.Data;
using Television_API.Models;


namespace Television_API.Repositories
{
    public interface IActorRepository
    {
        Task<Actor> GetActorAsync(int actorId);
        Task<IEnumerable<ActorDto>> GetActorsAsync();
        Task<IEnumerable<TVShowDto>> GetTVShowFromActorAsync(int actorId);
    }

    public class ActorRepository : IActorRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ActorRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Actor> GetActorAsync(int actorId)
        {
            var actor = await _context.Actors
                .Include(a => a.TvShows)
                .FirstOrDefaultAsync(a => a.Id == actorId);
            return actor;
        }

        public async Task<IEnumerable<ActorDto>> GetActorsAsync()
        {
            var actors = await _context.Actors.ToListAsync();
            return _mapper.Map<List<ActorDto>>(actors);
        }

        public async Task<IEnumerable<TVShowDto>> GetTVShowFromActorAsync(int actorId)
        {
            var actor = await GetActorAsync(actorId);
            return _mapper.Map<IEnumerable<TVShowDto>>(actor!.TvShows);
        }
    }
}
