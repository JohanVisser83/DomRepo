using Microsoft.AspNetCore.Http;
using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblCommunityTemporaryMember")]



 public class CommunityTemporaryMember : BaseEntity
 {
    public long CommunityId { get; set; }

    public string? FirstName { get; set; }   

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public string? Profile { get; set; }

    public long CustomerId { get; set; }

    public string AffiliateCode { get; set; }

    public override void ApplyKeys()
    {

    }
}

