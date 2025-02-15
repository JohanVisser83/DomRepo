using Microsoft.AspNetCore.Http;
using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCollectionAggregate")]

public class CollectionAggregate : BaseEntity
{
    
    public long? CustomerId { get; set; }
    public string title { get; set; }
    public long? CommunityId { get; set; }

    public long? EventOrganiser { get; set; }
    public string? OrganizerName { get; set; }
    public string? OrganizerProfilePic { get; set; }
    public string? OrganizerMobile { get; set; }
    public int? IsPaid { get; set; }
    public long? LinkedCustomerId { get; set; }
    public long? TransactionId { get; set; }

    public long? GroupId { get; set; }
    public decimal Amount { get; set; }
    public long? InvitationId { get; set; }
    public string? InviteeName { get; set;}
    public string? InviteeProfilePic { get; set; }
    public int? DaysLeft { get; set; }

    public DateTime EventStartDate { get; set; }
    public TimeSpan? EventStartTime { get; set; }

    public IFormFile? Mediafile { get; set; }

    public DateTime ExpirydateCollection { get; set; }

    public TimeSpan Expirytimecollection { get; set; }

    public DateTime Scheduledeliverydate { get; set; }

    public TimeSpan? Scheduleddeliverytime { get; set; }

    public string Description { get; set; }

    public string AccountMedia { get; set; }

    public bool? IsClosed { get; set; }

    public string? Type { get; set; }

    public string? GroupName { get; set; }

    public long ? Individual { get; set; }

    public DateTime EventEndDate { get; set; }
    public TimeSpan EventEndTime { get; set; }

    public string CommunityName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Name { get; set; }

    public string? EventOrganiserName { get; set; }


    public string Email { get; set; }

    public long TotalMemberAtCreation { get; set; }
    public long TotalPaid { get; set; }

    public decimal AmountCollected { get; set; }

    public string? CurrencyCode { get; set; }

    public override void ApplyKeys()
    {

    }
}



