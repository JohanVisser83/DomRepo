using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblVisitors")]
public class Visitor : BaseEntity
{
    public long VisitorId { get; set; } 
    public string Plan { get; set; }
    public decimal Price { get; set; }

    public override void ApplyKeys()
    {

    }
}


