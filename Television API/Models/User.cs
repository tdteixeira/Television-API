using System.ComponentModel.DataAnnotations;

namespace Television_API.Models
{
    public class User
    {
        [Key]
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public ICollection<TVShow> FavoriteShows { get; set; }
    }

    public class UserRequestDto
    {
        public string Username {  set; get; }
        public string Password { get; set; }
    }

    public class UserDto
    {
        public string Username { get; set; }
        //public ICollection<TVShowDto> FavoriteShows { get; set; }
    }
}
