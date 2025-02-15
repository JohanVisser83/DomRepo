using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblFormStandards")]

public class FormStandards : BaseEntity
{
    public string? Name { get; set; }
    public long Position { get; set; }
    public string? About { get; set; }
    public string? ProfileImage { get; set; }
    public string? Email { get; set; }
    public string? Contact { get; set; }
    public bool IsExcecutive { get; set; }
    public long? OrgId { get; set; }



    public override void ApplyKeys()
    {

    }
}