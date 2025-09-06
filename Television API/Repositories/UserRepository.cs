using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Television_API.Data;
using Television_API.Models;



namespace Television_API.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetUsers();
        Task<bool> RegisterUser(UserRequestDto request);
        Task<string> LoginUser(UserRequestDto request);

        Task<IEnumerable<TVShowDto>> GetFavoriteShows(string username);
        Task<bool> AddFavoriteShow(string username, int showId);
        Task<bool> RemoveFavoriteShow(string username, int showId);
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public UserRepository(AppDbContext context, IMapper mapper, IConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
        }

        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<string> LoginUser(UserRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.username == request.username);
            if (user == null)
                return null;

            using var hmac = new HMACSHA512(user.passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.password));

            if (!computedHash.SequenceEqual(user.passwordHash))
                return null;

            var token = GenerateJwtToken(user);// Função que gera o JWT
            return token;
        }

        public async Task<bool> RegisterUser(UserRequestDto request)
        {
            var existingUser = await _context.Users.FindAsync(request.username);
            if (existingUser != null)
            {
                return false; // Username is taken
            }

            using var hmac = new HMACSHA512();
            var user = new User
            {
                username = request.username,
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.password)),
                passwordSalt = hmac.Key,
                favoriteShows = new List<TVShow>()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddFavoriteShow(string username, int showId)
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

        public async Task<IEnumerable<TVShowDto>> GetFavoriteShows(string username)
        {
            var user = await _context.Users
                .Include(u => u.favoriteShows)
                .FirstOrDefaultAsync(u => u.username == username);

            if (user == null)
                return null;
            var favorites = user.favoriteShows;
            return _mapper.Map<IEnumerable<TVShowDto>>(favorites);

        }

        public async Task<bool> RemoveFavoriteShow(string username, int showId)
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

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.username),
            };

            var keyString = Environment.GetEnvironmentVariable("JWT_KEY");

            if (string.IsNullOrWhiteSpace(keyString) || keyString == "USE_ENV_VAR")
                throw new InvalidOperationException("JWT key is missing or invalid. Set the JWT_KEY environment variable.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
