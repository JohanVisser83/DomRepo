using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCommunityJourney")]

public class CommunityJourney : BaseEntity
{
    public long OrgId { get; set; }
    public long DriverId { get; set; }
    public string Title { get; set; }
    public string? Time { get; set; }
    public string? Date { get; set; }
    public bool? IsRecurring { get; set; }


    public override void ApplyKeys()
    {

    }
}
