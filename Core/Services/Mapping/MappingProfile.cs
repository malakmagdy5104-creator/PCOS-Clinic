using AutoMapper;
using Domain.Entities;
using Shared.DTOs.AuthDto;

namespace Services.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile() 
        {
        CreateMap<RegisterDto,ApplicationUser>()
          .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
          .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.UserName));
        }
    }
}
