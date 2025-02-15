using Microsoft.AspNetCore.Http;


namespace Circular.Core.DTOs
{
    public class CommunityTemporaryMemberDTO : BaseEntityDTO
    {

        public long CommunityId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string Mobile { get; set; }

        public IFormFile? MediaFile { get; set; }

        public string? Profile { get; set; }

        public bool? loginflow { get; set; }

        public string Otp { get; set; }

        public long CustomerId { get; set; }

        public string AffiliateCode { get; set; }
    }
}
