using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblMasterGroups_Community")]

public class MasterGroups_Community : BaseEntity
{
    public string? GroupName { get; set; }
    public string? GroupDesc { get; set; }


    public override void ApplyKeys()
    {

    }
}
