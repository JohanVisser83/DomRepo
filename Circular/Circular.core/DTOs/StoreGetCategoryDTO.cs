namespace Circular.Core.DTOs
{
    public class CheckOutDTO
    {
        public long? Storeid { get; set; }

        public decimal? TotalAmount { get; set; }
        public long? noofitems { get; set; }
        public long? OrderForId { get; set; }
        public long? loggedinUser { get; set; }
        public string PromoCode { get; set; }


    }

    public class UpdateCartDTO
    {
        public long? ProductId { get; set; }

        public decimal? Amount { get; set; }
        public long? Quantity { get; set; }
        public long? Storeid { get; set; }
        public long? loggedinUser { get; set; }

    }

    public class collectedOrderDTO
    {
        public long? loggedinuser { get; set; }
        public long? Collected { get; set; }
        public long? OrderId{ get; set;}

    }

    public class collectDTO
    {
        public long OrderId { get; set; }
        public bool? IsCollected { get; set;}
        public long CollectedBy { get; set; }
       

    }

    public class GetStoresDTO
    {
        public long? CommunityId { get; set; }
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

    }

}
