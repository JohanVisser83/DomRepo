using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public class EventNotificationDTO
    {
        public long? CommunityId { get; set; }
        public long? Id { get; set; }
        public long? GroupId { get; set; }

        public string? Title { get; set; }
        public decimal? TicketPrice { get; set; }
        public string? CommunityName { get; set; }
        public long? CreatedBy { get; set; }
    }
}
