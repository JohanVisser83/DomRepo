using Circular.Core.Entity;
namespace Circular.Core.DTOs
{
    public class SponsorInformationDTO
    {
        public SponsorInformationDTO()
        {
            if (SectionDetails == null)
                SectionDetails = new List<SponsorSectionDetailsDTO>();
        }
        public long? SponsorId { get; set; }

        public long? CommunityId { get; set; }
        public string? Title { get; set; }
        public string? SponsorLogo { get; set; }
        public string? CoverImage { get; set; }
        public string? SponsorDecription { get; set; }
        public string? EmailButton { get; set; }
        public string? CallButton { get; set; }
        public string? Website { get; set; }
        public string? BusinessAddress { get; set; }
        public bool? LogoOnly { get; set; }
        public List<SponsorSectionDetailsDTO> SectionDetails { get; set; }

    }
}
