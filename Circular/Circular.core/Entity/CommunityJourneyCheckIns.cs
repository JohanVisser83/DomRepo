using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCommunityJourneyCheckIns")]

public class CommunityJourneyCheckIns : BaseEntity
{
    public long? JourneyId { get; set; }
    public long? CustomerId { get; set; }
    public decimal CustLang { get; set; }
    public decimal CustLong { get; set; }



    public override void ApplyKeys()
    {

    }
}
