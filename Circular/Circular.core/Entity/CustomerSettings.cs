using RepoDb.Attributes;
using System.ComponentModel;

namespace Circular.Core.Entity;
[Map("tblCustomerSettings")]
public class CustomerSettings : BaseEntity
{
    public long? CustomerId { get; set; }
    public string? key { get; set; }
    
    
    [DefaultValue("01/01/2020")]
    public string? value { get; set; }
    public string? Description { get; set; }


    public override void ApplyKeys()
    {

    }
}