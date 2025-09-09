using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using Television_API.Data;
using Television_API.Models;



namespace Television_API.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<IEnumerable<UserDto>> GetPagedUsersAsync(PaginationParams p);
        Task<User> GetUserAsync(string  username);
        Task<bool> AddUserAsync(User user);
        Task<IEnumerable<TVShowDto>> GetPagedFavoriteShowsAsync(PaginationParams p, string username);
        Task<bool> AddFavoriteShowAsync(string username, int showId);
        Task<bool> RemoveFavoriteShowAsync(string username, int showId);
        Task<IEnumerable<UserDto>> SearchUserAsync(PaginationParams p, string queryString);
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
                .Include(u => u.FavoriteShows)
                .FirstOrDefaultAsync(u => u.Username == username);
            var show = await _context.TVShows.FindAsync(showId);
            if (show == null || user!.FavoriteShows.Contains(show)) 
            {
                return false;
            }
            user!.FavoriteShows.Add(show);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TVShowDto>> GetPagedFavoriteShowsAsync(PaginationParams p,string username)
        {
            var user = await _context.Users
                .Include(u => u.FavoriteShows)
                    .ThenInclude(s => s.Episodes)
                .Include(u => u.FavoriteShows)
                    .ThenInclude(s => s.FavoritedByUsers)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null;
            var favorites = user.FavoriteShows
                .Skip((p.PageNumber-1)*p.PageSize)
                .Take(p.PageSize)
                .ToList();
            return _mapper.Map<IEnumerable<TVShowDto>>(favorites);
        }

        public async Task<bool> RemoveFavoriteShowAsync(string username, int showId)
        {
            var user = await GetUserAsync(username);
            var show = await _context.TVShows.FindAsync(showId);
            if (show == null || !user!.FavoriteShows.Contains(show))
            {
                return false;
            }
            user!.FavoriteShows.Remove(show);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> GetUserAsync(string username)
        {
            var user = await _context.Users
                 .Include(u => u.FavoriteShows)
                 .FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetPagedUsersAsync(PaginationParams p)
        {
            var users = await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Username)
                .Skip((p.PageNumber - 1) * p.PageSize)
                .Take(p.PageSize)
                .ToListAsync();
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<IEnumerable<UserDto>> SearchUserAsync(PaginationParams p, string queryString)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryString))
                query = query.Where(u => EF.Functions.Like(u.Username.ToLower(), $"%{queryString.ToLower()}%"));

            var users = await query
            .AsNoTracking()
            .OrderBy(u => u.Username)
            .Skip((p.PageNumber - 1) * p.PageSize)
            .Take(p.PageSize)
            .ToListAsync();
            return _mapper.Map<List<UserDto>>(users);
        }
    }
}
