using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblInvoiceItem")]

public class InvoiceItem : BaseEntity
{
    public long? InvoiceId { get; set; }
    public string? Product { get; set; }
    public long? Quantity { get; set; }
    public decimal? Rate { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? DueDate { get; set; }
    public long? CustomerId { get; set; }
    public string? ViewPdf { get; set; }
    public long? InvoiceStatusId { get; set; }


    public override void ApplyKeys()
    {

    }
}
