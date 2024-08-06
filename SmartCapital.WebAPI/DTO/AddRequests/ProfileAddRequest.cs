using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.Application.DTO.AddRequests
{
    public class ProfileAddRequest
    {
        public string ProfileName { get; set; } = null!;
        public decimal? ProfileOpeningBalance { get; set; }

        public Profile ToProfile()
        {
            return new Profile
            {
                ProfileName = ProfileName,
                ProfileOpeningBalance = ProfileOpeningBalance
            };
        }
    }
}
