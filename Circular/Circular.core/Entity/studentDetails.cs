using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblstudentDetails")]

public class studentDetails : BaseEntity
{
    public long? CustomerId { get; set; }
    public long? ClassNoid { get; set; }
    public string? StudentNo { get; set; }
    public long? Teacherid { get; set; }
    public long? HouseId { get; set; }


    public override void ApplyKeys()
    {

    }
}