using Microsoft.AspNetCore.Http;

namespace Circular.Core.DTOs
{
    public class MessageScheduleDTO
    {
        public long FromId { get; set; }
        public long ToId { get; set; }

        public long CommunityId { get; set; }
        public string? MessageExchangeCode { get; set; }
        public long MessageTypeId { get; set; }

        public IFormFile? Mediafile { get; set; }
        public string? Message { get; set; }
        public string? MessageMedia { get; set; }
        public string? MessageMediaThumbnail { get; set; }
        public long? ReferenceId { get; set; }

        public DateTime Schedule { get; set; }


    }
}
