using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblSponsorSectionCircle")]
public class SponsorSectionCircle : BaseEntity
{
    public long? SectionId { get; set; }
    public string? Title { get; set; }
    public string? Numbers { get; set; }
    public long? SponsorId { get; set; }
    public override void ApplyKeys()
    {

    }
}