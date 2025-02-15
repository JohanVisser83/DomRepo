using RepoDb.Attributes;
namespace Circular.Core.Entity
{
    [Map("tblTransactions")]
    public class Transactions : BaseEntity
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
        public string? CampaignCode { get; set; }
        public string? PaymentDesc { get; set; }
        public decimal? CurrentWalletBalance { get; set; }
        public long? RequestTransId { get; set; }
        public long? CollectionReqId { get; set; }
        public long? EventId { get; set; }
        public long? StoreId { get; set; }
        public long? CustomerId { get; set; }
        public long? OrderId { get; set; }
        public long? RefundId { get; set; }
        public string? Title { get; set; }
        public long? CommunityId { get; set; }

		public long? TicketDayId { get; set; }
        public long? Quantity { get; set; }
        public long? Fundhub { get; set; }
        public int? WalletAmount { get; set; }

        public string? StoreName { get; set; }
        public long? ReferenceId { get; set; }
        public String? ReferenceType { get; set; }

        public override void ApplyKeys()
        {

        }
    }
    public class TransactionRequest
    {
        public long? CommunityId { get; set; }
        public long TransactionTypeId { get; set; }
        public string? Currency { get; set; }
        public long TransactionFrom { get; set; }
        public string? MobileNumber { get; set; }
        public decimal Amount { get; set; }
        public string? RefundNote { get; set; }
        public string? PaymentDesc { get; set; }
        public long? TransactionTo { get; set; } = 0;
        public bool IsPaymentFromMessageModule { get; set; }
        public long TransactionStatusId { get; set; } = 101;
        public long? RefundId { get; set; }
        public long? RequestedTransactionId { get; set; } = 0;
        public decimal? ServiceCharges { get; set; } = 0;
        public long? OrderId { get; set; }
        public string? PaymentGateway { get; set; }

        public long? ReferenceId { get; set; }

        public string? ReferenceType { get; set; }



    }

    public class WalletTransactions
    {
        public long Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionAmount { get; set; }
        public long TransactionToId { get; set; }
        public string TransactionToFullName { get; set; }
        public string TransactionToProfile { get; set; }
        public long TransactionTypeId { get; set; }
        public string TransactionTypeName { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionText { get; set; }
    }






    public class TransactionSubscription
    {
        public long? CommunityId { get; set; }
        public long TransactionFromID { get; set; }
        public long TransactionToID { get; set; }
        public string TransactionFromName { get; set; }
        public string TransactionToName { get; set; }
       
        public string? CommunityName { get; set; }
        public DateTime TransactionDate { get; set; }

        public decimal Amount { get; set; }
        public string Description { get; set; } 
 
        public long TransactionTypeId { get; set; }

        public string ReferenceType { get; set; }   


    }
}