using AutoMapper;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.DTO.AddRequests;
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
    }
}
