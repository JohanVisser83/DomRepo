using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblMerchantAddressAndLegalInfo")]

public class MerchantAddressAndLegalInfo : BaseEntity
{
    public long CustomerId { get; set; }
    public string? StreetAddress { get; set; }
    public string? BuildingDetails { get; set; }
    public string? Suburb { get; set; }
    public long? City { get; set; }
    public long? Province { get; set; }
    public string? PostalCode { get; set; }
    public long? BusinessType { get; set; }
    public string? IncomeSource { get; set; }
    public string? MonthlyTurnOver { get; set; }



    public override void ApplyKeys()
    {

    }
}
