using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblTransportPassQRs")]

public class TransportPassQRs : BaseEntity
{
    public long? CommunityId { get; set; }
    public string? Title { get; set; }
    public decimal? Price { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? QR { get; set; }
    public bool? IsPass { get; set; }


    public override void ApplyKeys()
    {

    }
}
