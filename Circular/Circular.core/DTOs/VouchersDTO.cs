namespace Circular.Core.DTOs
{
    public class VouchersDTO
    {
        public long? CompanyId { get; set; }
        public string? VoucherName { get; set; }
        public decimal? VoucherAmount { get; set; }
        public string? VoucherDesc { get; set; }
    }
}
