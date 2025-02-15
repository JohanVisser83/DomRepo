using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblSponsorSectionfields")]
public class SponsorSectionfields : BaseEntity
{
    public SponsorSectionfields()
    {
        if (SectionCircles == null)
            SectionCircles = new List<SponsorSectionCircle>();
        if (SectionImages == null)
            SectionImages = new List<SponsorSectionImages>();
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

    public List<SponsorSectionCircle> SectionCircles { get; set; }
    public List<SponsorSectionImages> SectionImages { get; set; }

    public override void ApplyKeys()
    {

    }
}
