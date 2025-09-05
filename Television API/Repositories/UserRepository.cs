using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Television_API.Data;
using Television_API.Models;


namespace Television_API.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetUsers();
        Task<bool> RegisterUser(UserRegistrationDto request);
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

        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<bool> RegisterUser(UserRegistrationDto request)
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
                passwordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
