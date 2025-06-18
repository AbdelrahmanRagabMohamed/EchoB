using AutoMapper;
using EchoB.Application.DTOs;
using EchoB.Application.DTOs.User;
using EchoB.Domain.Entities;

namespace EchoB.Application.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EchoBUser, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfilePictureUrl != null ? src.ProfilePictureUrl : null))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio != null ? src.Bio : null));

            CreateMap<CreateUserDto, EchoBUser>()
                .ConstructUsing(src => new EchoBUser(
                    new Domain.ValueObjects.FullName(src.FullName),
                    src.UserName
                ));
        }
    }
}

