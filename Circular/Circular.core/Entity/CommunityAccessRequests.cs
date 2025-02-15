using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblCommunityAccessRequests")]
   public  class CommunityAccessRequests : BaseEntity
   {
    public long CustomerId { get; set; }    
    public long CommunityId { get; set;}
    public long StatusId { get; set; }

    public string FirstName{ get;set;}
    public string Email { get; set; }
    public string Mobile { get; set; }
    public DateTime RequestedDate { get; set; }
    public long? Id { get; set; }
    public string CommunityName { get; set; }

    public override void ApplyKeys()
    {

    }
}

