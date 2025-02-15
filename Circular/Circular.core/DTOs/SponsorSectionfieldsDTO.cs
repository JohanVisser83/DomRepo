using Circular.Core.Entity;
namespace Circular.Core.DTOs
{
    public class SponsorSectionfieldsDTO
    {
        public SponsorSectionfieldsDTO()
        {
            if (SectionCircles == null)
                SectionCircles = new List<SponsorSectionCircleDTO>();
            if (SectionImages == null)
                SectionImages = new List<SponsorSectionImagesDTO>();
        }
        public long? SectionId { get; set; }
        public string? TitleMainPage { get; set; }
        public string? AmtMainPage { get; set; }
        public string? ImageMainPage { get; set; }
        public string? BoldTitleItemPage { get; set; }
        public string? GreyTextItemPage { get; set; }
        public string? DescriptionItemPage { get; set; }
        public string? Image { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Whatsapp { get; set; }
        public string? Weblink { get; set; }
        public string? Locationlink { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Address { get; set; }
        public string? OfferPrice { get; set; }
        public string? AgentName { get; set; }
        public string? Agentprofile { get; set; }
        public long? SponsorId { get; set; }

        public List<SponsorSectionCircleDTO> SectionCircles { get; set; }
        public List<SponsorSectionImagesDTO> SectionImages { get; set; }
    }
}
