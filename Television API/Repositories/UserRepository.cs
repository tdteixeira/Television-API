using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Television_API.Data;
using Television_API.Models;



namespace Television_API.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<IEnumerable<TVShowDto>> GetFavoriteShowsAsync(string username);
        Task<bool> AddFavoriteShowAsync(string username, int showId);
        Task<bool> RemoveFavoriteShowAsync(string username, int showId);
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public UserRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<bool> AddFavoriteShowAsync(string username, int showId)
        {
            var user = await _context.Users
                .Include(u => u.favoriteShows)
                .FirstOrDefaultAsync(u => u.username == username);
            var show = await _context.TVShows.FindAsync(showId);
            if (show == null || user!.favoriteShows.Contains(show)) 
            {
                return false;
            }
            user!.favoriteShows.Add(show);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TVShowDto>> GetFavoriteShowsAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.favoriteShows)
                .FirstOrDefaultAsync(u => u.username == username);

            if (user == null)
                return null;
            var favorites = user.favoriteShows;
            return _mapper.Map<IEnumerable<TVShowDto>>(favorites);

        }

        public async Task<bool> RemoveFavoriteShowAsync(string username, int showId)
        {
            var user = await _context.Users
                 .Include(u => u.favoriteShows)
                 .FirstOrDefaultAsync(u => u.username == username);
            var show = await _context.TVShows.FindAsync(showId);
            if (show == null || !user!.favoriteShows.Contains(show))
            {
                return false;
            }
            user!.favoriteShows.Remove(show);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
