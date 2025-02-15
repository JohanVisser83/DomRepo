using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblSponsorSectionImages")]
public class SponsorSectionImages : BaseEntity
{
    public long? SectionId { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public long? SponsorId { get; set; }


    public override void ApplyKeys()
    {

    }
}
