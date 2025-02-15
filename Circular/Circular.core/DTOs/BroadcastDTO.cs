
using Microsoft.AspNetCore.Http;




namespace Circular.Core.DTOs
{
    public class BroadcastDTO : BaseEntityDTO
    {
        public long CommunityId { get; set; }

        public long CustomerId { get; set; }

        public long IsGroup { get; set; }

        public long ReferenceId { get; set; }

        public string? Title { get; set; }

        public string? Text { get; set; }

        public string? MessageMedia { get; set; }

        public IFormFile? Mediafile { get; set; }
        public string? MessageMediaThumbnail { get; set; }

        public DateTime ScheduleDateTime { get; set; }

        public DateTime? ScheduleTime { get; set; }
        public string? Type { get; set; }
        public long? SelectedId { get; set; }

       

        public long? MessageTypeId { get; set; }

        public string? CommunityName { get; set; }


        public class BroadcastSummaryDetails 
        {
            public string? Text { get; set; }
            public string? Title { get; set; }
            public string? MessageMedia { get; set; }

           
        }



    }


    public class PostBroadcast
    {
        public long CommunityId { get; set; }

        public long CustomerId { get; set; }

        public long IsGroup { get; set; }

        public long ReferenceId { get; set; }

        public string? Title { get; set; }

        public string? Text { get; set; }

        public string? MessageMedia { get; set; }

        public string? MessageMediaThumbnail { get; set; }

        public DateTime ScheduleDateTime { get; set; }

        public DateTime? ScheduleTime { get; set; }
        public string? Type { get; set; }

        public long? MessageTypeId { get; set; }

        public string? CommunityName { get; set; }
    }
}
