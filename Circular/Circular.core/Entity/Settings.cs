using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblSettings")]
public class Settings : BaseEntity
{
    public long? CommunityId { get; set; }
    public string? Key { get; set; }
    public string? Value { get; set; }
    public string? Description { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal? FixedValue { get; set; }


    public override void ApplyKeys()
    {
        
    }
}