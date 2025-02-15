using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblSponsorSectionDetails")]
public class SponsorSectionDetails : BaseEntity
{
    public SponsorSectionDetails()
    {
        if (SectionFields == null)
            SectionFields = new List<SponsorSectionfields>();
    }
    public int? TypeId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public long? SponsorId { get; set; }

    public List<SponsorSectionfields> SectionFields { get; set; }
    public override void ApplyKeys()
    {

    }
}
