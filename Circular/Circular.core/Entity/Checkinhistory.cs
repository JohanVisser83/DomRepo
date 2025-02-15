using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCheckinhistory")]

public class Checkinhistory : BaseEntity
{
    public long? CustomerId { get; set; }
    public DateTime? Date { get; set; }
    public TimeOnly? Timespan { get; set; }
    public decimal? Amount { get; set; }
    public string? Scannedfor { get; set; }
    public decimal? Distance { get; set; }
    public bool? IsFlexiPass { get; set; }


    public override void ApplyKeys()
    {

    }
}

