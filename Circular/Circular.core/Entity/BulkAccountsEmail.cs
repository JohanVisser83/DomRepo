using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblBulkAccountsEmail")]


    public class BulkAccountsEmail : BaseEntity
    {
     public long AccountId { get; set; }

    public long IsGroup { get; set; }

    public long GroupId { get; set; }

    public bool IsSent { get; set; }
    public long communityId { get; set; }

    public string? CommunityName { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? ExpirydateCollection { get; set; }

    public DateTime Scheduledeliverydate { get; set; }

    public TimeSpan? Scheduleddeliverytime { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Currency { get; set; }

    public long Accountcustomer { get; set; }

    public override void ApplyKeys()
    {

    }
}

