using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Television_API.Data;
using Television_API.Models;

namespace Television_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActorsController: ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ActorsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetActors")]
        public async Task<IEnumerable<ActorDto>> GetActors()
        {
            var actors = await _context.Actors
                .ToListAsync();
            return _mapper.Map<List<ActorDto>>(actors);
        }

    }
}
