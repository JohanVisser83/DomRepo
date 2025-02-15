using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblSponsorInformation")]
public class SponsorInformation : BaseEntity
{
    public SponsorInformation()
    {
        if (SectionDetails == null)
            SectionDetails = new List<SponsorSectionDetails>();
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

    public List<SponsorSectionDetails> SectionDetails { get; set; }


    public override void ApplyKeys()
    {

    }
}

