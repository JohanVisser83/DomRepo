namespace Circular.Core.DTOs
{
    public class TransactionsDTO
    {
        public long? TransactionTypeId { get; set; }
        public string? TransactionCode { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public DateTime? TransactionDate { get; set; }
        public long? TransactionStatusId { get; set; }
        public long? TransactionFrom { get; set; }
        public long? TransactionTo { get; set; }
        public string? PaymentGatewayId { get; set; }
        public string? PaymentTranId { get; set; }
        public long? PaymentGatewayStatusId { get; set; }
        public decimal? TipAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? ServiceFee { get; set; }
        public long? ReferenceId { get; set; }
        public String? ReferenceType { get; set; }
        public string? CampaignCode { get; set; }
        public string? PaymentDesc { get; set; }
        public decimal? CurrentWalletBalance { get; set; }
        public long? RequestTransId { get; set; }
        public long? CollectionReqId { get; set; }
        public long? EventId { get; set; }
        public long? StoreId { get; set; }
        public long? RefundId { get; set; }
        public long? CustomerId { get; set; }
        public long? OrderId { get; set; }
        public string? Title { get; set; }
        public long? CommunityId { get; set; }

        public long? Fundhub { get; set; }

        public int? WalletAmount { get; set; }  

        public string? StoreName { get; set;}   
        public long Quantity { get; set; }
    }

    public class TransactionRequestDTO
    {
        public long? CommunityId { get; set; }
        public long? TransactionTypeId { get; set; }
        public string? Currency { get; set; }
        public long? TransactionFrom { get; set; }
        public string? MobileNumber { get; set; }
        public string? RefundNote { get; set; }
        public decimal? Amount { get; set; }
        public long? TransactionTo { get; set; } = 0;
        public bool IsPaymentFromMessageModule { get; set; }
        public long? RefundId { get; set; }
        public long? RequestedTransactionId { get; set; } = 0;
        public string? PaymentDesc { get; set; }

        public decimal? ServiceCharges { get; set; } = 0;

        public long? OrderId { get; set; }

        public long? TicketDayId { get; set; }

        public long? Quantity { get; set; }

        public string? PaymentGateway { get; set; }
        public string? SuccessUrl { get; set; }
        public string? FailureUrl { get; set; }

        public decimal? FixedCharge { get; set; }

        public long? ReferenceId { get; set; }

        public string? ReferenceType { get; set; }

        public string? CurrencyCode { get; set; } 
    }

    public class TransactionDetailDTO
    {
        public long TransactionId { get; set; }
        public long CustomerId { get; set; }
    }
	public class DeclineTransactionDTO
	{
		public long TransactionId { get; set; }
		public long FromCustomerId { get; set; }
		public long ToCustomerId { get; set; }
		public decimal? Amount { get; set; }

	}

    public class EmailDetailDTO
    {
        public long TransactionId { get; set; }
        public long CustomerId { get; set; }
        public string EmailId { get; set; }
    }
}
