using AutoMapper;
using Television_API.Models;
namespace Television_API.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Episode, EpisodeDto>();
            CreateMap<TVShow, TVShowDto>()
            .ForMember(dest => dest.NumberOfFavorites,
                       opt => opt.MapFrom(src => src.FavoritedByUsers.Count))
            .ForMember(dest => dest.NumberOfEpisodes,
                       opt => opt.MapFrom(src => src.Episodes.Count));
            CreateMap<Actor, ActorDto>();
            CreateMap<User, UserDto>();
        }
    }
}
