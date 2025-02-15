using RepoDb.Attributes;
using System.Diagnostics.Contracts;

namespace Circular.Core.Entity;

[Map("tblFundraiserCollection")]

public class FundraiserCollection : BaseEntity
{
    public long FundraiserId { get; set; }
    public long UserId { get; set; }
    public decimal Amount { get; set; }
    public int IsCollected { get; set; }
    public long TransactionId { get; set; }


    public override void ApplyKeys()
    {

    }
    public long Id { get; set; }
    public long FundraiserCollectionId { get;set;}
    public decimal CollectedAmount { get; set; }
    public string PayeeName { get; set; }
    public bool MarkCollected { get; set; }
    public decimal ProductPrice { get; set; }
    public DateTime Date { get; set; }
    public string Mobile { get; set; }
    public string RecipientName { get; set; }
    public string RecipientDate{get;set;}
    public string Title { get; set; }
    public long TotalCollected { get; set; }

}