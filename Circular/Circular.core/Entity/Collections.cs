using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCollections")]

public class Collections : BaseEntity
{
    public long? CollectionReqId { get; set; }
    public long? FromId { get; set; }
    public decimal Amount { get; set; }
    public long? LinkedCustomerId { get; set; }
    public long? Communityid { get; set; }
    public long? TransactionId { get; set; }


    

    public override void ApplyKeys()
    {

    }
}


