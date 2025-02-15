namespace Circular.Core.DTOs
{
    public class JobsDTO : BaseEntityDTO
    {
        public long CommunityId { get; set; }
        public long CustomerId { get; set; }
        public long BusinessId { get; set; }
        public long CategoryId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? CompanyEmail { get; set; }
        public string? ContactNumber { get; set; }
        public long? CompanySize { get; set; }
        public string? YourName { get; set; }
        public string? Position { get; set; }
        public string JobTitle { get; set; }
        public string JobDesc { get; set; }
        public string? Location { get; set; }
        public int WorkTypeId { get; set; }
        public string? CoverPhoto { get; set; }
        public string? AttachJobSpec { get; set; }
        public bool IsFilled { get; set; }
        public bool? IsApproved { get; set; }
        public long ViewCount { get; set; } = 0;
        public long ApplicantCount { get; set; } = 0;

        public string? BusinessLogo { get; set; }

    }
}
