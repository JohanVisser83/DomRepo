using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblRefund")]
public class Refund : BaseEntity
{
    public long CommunityId { get; set; }
    public long UserId { get; set; }
    public decimal Amount { get; set; }
    public String RefundNote { get; set; }
    public override void ApplyKeys()
    {

    }
}

