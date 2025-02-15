using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblbulkEventEmail")]

public class BulkEventEmail : BaseEntity
{
    public long EventId { get; set; }

    public long IsGroup { get; set; }

    public long GroupId { get; set; }

    public bool IsSent { get; set; }
    public long communityId { get; set; }

    public string? CommunityName { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? ExpirydateCollection { get; set; }

    public DateTime? Scheduledeliverydate { get; set; }

    public TimeSpan? Scheduleddeliverytime { get; set; }

    public string? Title { get; set; }
    public string? OrganiserName { get; set; }
    public DateTime? EventStartDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public string? PaymentStatus { get; set; }

    public long Eventcustomer { get; set; }

    public override void ApplyKeys()
    {

    }


}