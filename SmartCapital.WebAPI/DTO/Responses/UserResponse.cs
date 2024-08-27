using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.DTO.Responses
{
    public class UserResponse
    {
        public string UserName { get; set; } = null!;

        public string UserPassword { get; set; } = null!;

        public DateTime UserCreationDate { get; set; }
    }

    public static class UserExtensions
    {
        public static UserResponse ToUserResponse(this User user)
        {
            return new UserResponse
            {
                UserCreationDate = user.UserCreationDate,
                UserName = user.UserName,
                UserPassword = user.UserPassword
            };
        }
    }
}
