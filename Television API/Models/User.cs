using System.ComponentModel.DataAnnotations;

namespace Television_API.Models
{
    public class User
    {
        [Key]
        public string username { get; set; }
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }
        //public ICollection<TVShow> favoriteShows { get; set; }
    }

    public class UserRequestDto
    {
        public string username {  set; get; }
        public string password { get; set; }
    }

    public class UserDto
    {
        public string username { get; set; }
        //public ICollection<TVShowDto> favoriteShows { get; set; }
    }
}
