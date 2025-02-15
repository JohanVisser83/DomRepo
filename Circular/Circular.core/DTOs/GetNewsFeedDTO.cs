namespace Circular.Core.DTOs
{
    public class GetNewsFeedDTO
    {
        public long CustomerId { get; set; }
        public long Feedid { get; set; }
    }

    public class GetArticleDetailsDTO
    {
        public long FeedId { get; set; }    
        public long Userid { get; set; }
    }

    public class GetAdvertisement
    {
        public long CommunityId { get; set; }
        public long AdvertisementId { get; set; }

    }
}
