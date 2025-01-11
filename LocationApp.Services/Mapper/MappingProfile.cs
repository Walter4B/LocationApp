using AutoMapper;
using LocationApp.Model.Core;
using LocationApp.Model.DataTransferObjects;

namespace LocationApp.Services.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.First().ToString().ToUpper() + src.Name.Substring(1).Replace('_', ' ')));
            CreateMap<Location, LocationDto>()
                .ForMember(dest => dest.IsFavorite, opt => opt.MapFrom(src => src.Users.Any()));
        }
    }
}
