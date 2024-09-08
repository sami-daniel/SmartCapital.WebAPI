// none

using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.DTO.Responses;

public class UserResponse
{
    public string UserName { get; set; } = null!;
    public DateTime UserCreationDate { get; set; }
    public virtual ICollection<ProfileResponse> Profiles { get; set; } = [];
}

public static class UserResponseExtensions
{
    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse
        {
            UserName = user.UserName,
            UserCreationDate = user.UserCreationDate,
            Profiles = user.Profiles.Select(p => p.ToProfileResponse()).ToList()
        };
    }
}
