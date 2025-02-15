using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblVouchers")]

public class Vouchers : BaseEntity
{
    public long? CompanyId { get; set; }
    public string? VoucherName { get; set; }
    public decimal? VoucherAmount { get; set; }
    public string? VoucherDesc { get; set; }



    public override void ApplyKeys()
    {

    }
}
