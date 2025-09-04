using AutoMapper;
using Television_API.Models;
namespace Television_API.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Episode, EpisodeDto>();
            CreateMap<TVShow, TVShowDto>();
            CreateMap<Actor, ActorDto>();
        }
    }
}
