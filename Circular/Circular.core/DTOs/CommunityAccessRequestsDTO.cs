using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public  class CommunityAccessRequestsDTO
    {
        public long CustomerId { get; set; }

        public long CommunityId { get; set; }

        public long StatusId { get; set; }
    }
}
