using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("TblStorefrontSchedule")]
public  class StorefrontSchedule:BaseEntity
    {
    
    public string? Title { get; set; }
    public long? CommunityId { get; set; }
    public long? StoreId { get; set; }
    public DateTime? OpenTime { get; set; }
    public DateTime? ClosedTime { get; set; }

    public DateTime? OpenTimming { get; set; }
    public DateTime? ClosedTimming { get; set; }

    public string? Days { get; set; }
   
    public override void ApplyKeys()
    {

    }

}

