namespace Circular.Core.DTOs
{
    public class InvoiceDTO
    {
        public long? CustomerId { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? BillToAddress { get; set; }
        public string? CircularAccountNumberForPayment { get; set; }
        public string? ShipToAddress { get; set; }
        public string? PaymentMethodId { get; set; }
        public DateTime? Date { get; set; }
        public string? Curruncy { get; set; }
        public decimal? Price { get; set; }
        public long? PurchaseOrderId { get; set; }
        public string? Reference { get; set; }
        public string? Note { get; set; }
        public string? CASH { get; set; }
        public string? EFT { get; set; }
        public long? BankId { get; set; }
        public string? InvoiceNumber { get; set; }
    }
}
