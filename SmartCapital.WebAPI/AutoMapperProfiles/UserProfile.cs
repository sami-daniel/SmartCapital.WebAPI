using AutoMapper;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.DTO.AddRequests;
using SmartCapital.WebAPI.DTO.Responses;
using Profile = AutoMapper.Profile;

namespace SmartCapital.WebAPI.AutoMapperProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserAddRequest, User>()
               .ForMember(dest => dest.UserId, opt => opt.Ignore())
               .ForMember(dest => dest.UserCreationDate, opt => opt.Ignore())
               .ForMember(dest => dest.Profiles, opt => opt.Ignore());

        CreateMap<User, UserResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.UserCreationDate, opt => opt.MapFrom(src => src.UserCreationDate))
                .ForMember(dest => dest.Profiles, opt => opt.MapFrom(src => src.Profiles.Select(u => new UserResponse
                {
                    UserName = u.ProfileName,
                    UserCreationDate = u.ProfileCreationDate
                })));
    }
}
