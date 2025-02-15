using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblStaffClasses")]

public class StaffClasses : BaseEntity
{
    public long StaffId { get; set; }
    public long? ClassId { get; set; }

    public string StaffName { get; set; }


	public override void ApplyKeys()
    {

    }
}
