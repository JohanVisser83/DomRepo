using Circular.Core.Entity;
namespace Circular.Core.DTOs
{
    public class SponsorSectionDetailsDTO
    {
        public SponsorSectionDetailsDTO()
        {
            if (SectionFields == null)
                SectionFields = new List<SponsorSectionfieldsDTO>();
        }
        public int? TypeId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public long? SponsorId { get; set; }
        public string? Agentprofile { get; set; }
        public string? AgentName { get; set; }
        public string? OfferPrice { get; set; }
        public string? Address { get; set; }
        public List<SponsorSectionfieldsDTO> SectionFields { get; set; }

    }
}
