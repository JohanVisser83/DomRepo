using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public class CustomerSupportsDTO : BaseEntityDTO
    {
        public string? Code { get; set; }

        public long? CommunityId { get; set; }

        public long? CustomerId { get; set; }
    }
}
