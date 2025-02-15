using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public class ListRequestDTO
    {
        public long CommunityId { get; set; }
        public int UpcomingOrPast { get; set; }
        public bool IsGrouped { get; set; }
    }

}
