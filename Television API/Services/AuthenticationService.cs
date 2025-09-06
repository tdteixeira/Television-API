using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Television_API.Data;
using Television_API.Models;

namespace Television_API.Services
{
    public interface IAuthenticationService
    {
        Task<bool> RegisterUserAsync(UserRequestDto request);
        Task<string> LoginUserAsync(UserRequestDto request);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthenticationService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> LoginUserAsync(UserRequestDto request)
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

        public async Task<bool> RegisterUserAsync(UserRequestDto request)
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
