using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Television_API.Data;
using Television_API.Models;


namespace Television_API.Repositories
{
    public interface IActorRepository
    {
        Task<IEnumerable<ActorDto>> GetActors();
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

        public async Task<IEnumerable<ActorDto>> GetActors()
        {
            var actors = await _context.Actors.ToListAsync();
            return _mapper.Map<List<ActorDto>>(actors);
        }
    }
}
