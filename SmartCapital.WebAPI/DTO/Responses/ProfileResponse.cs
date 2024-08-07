using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.DTO.Responses
{
    public class ProfileResponse
    {
        public uint ProfileId { get; set; }

        public DateTime ProfileCreationDate { get; set; }

        public string ProfileName { get; set; } = null!;

        public decimal? ProfileOpeningBalance { get; set; }
    }

    public static class ProfileExtensions
    {
        public static ProfileResponse ToProfileResponse(this Profile profile)
        {
            return new ProfileResponse
            {
                ProfileId = profile.ProfileId,
                ProfileCreationDate = profile.ProfileCreationDate,
                ProfileOpeningBalance = profile.ProfileOpeningBalance,
                ProfileName = profile.ProfileName
            };
        }
    }
}
