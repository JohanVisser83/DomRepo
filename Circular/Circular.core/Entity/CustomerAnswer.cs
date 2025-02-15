using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCustomerAnswer")]

public class CustomerAnswer : BaseEntity
{
    public long? CustId { get; set; }
    public DateTime? Date { get; set; }
    public long? Questionid { get; set; }
    public long? Optionsid { get; set; }
    public string? Value { get; set; }
    public bool? IsSymptom { get; set; }


    public override void ApplyKeys()
    {

    }
}