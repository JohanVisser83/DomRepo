

namespace Circular.Core.DTOs
{
    public class StorefrontScheduleDTO
    {
       
        public string? Title { get; set; }
        public long? CommunityId { get; set; }
        public long? StoreId { get; set; }
        public DateTime? OpenTime { get; set; }
        public DateTime? ClosedTime { get; set; }

        public DateTime? OpenTimming { get; set; }
        public DateTime? ClosedTimming { get; set; }
        public string? Days { get; set; }

    }
	public class GetStore
	{
		public long StoreId { get; set; }
		public string? StoreName { get; set; }
		public string DisplayImage { get; set; }
		public long CommunityId { get; set; }
		public DateTime OpenTime { get; set; }
		public DateTime ClosedTime { get; set; }
		
		public int IsOpen { get; set; }
		

	}
}
