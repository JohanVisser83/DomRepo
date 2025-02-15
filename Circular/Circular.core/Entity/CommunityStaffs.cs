using Microsoft.AspNetCore.Http;
using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCommunityStaff")]

public class CommunityStaffs : BaseEntity
{
    public long Id { get; set; }
    public long? CommunityId { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public string? Email { get; set; }
    public string? Contact { get; set; }
    public string? ProfileImage { get; set; }
    public string? ProfileName { get; set; }
    public string? Image { get; set; }
    public string? About { get; set; }
          


    public override void ApplyKeys()
    {

    }
}
