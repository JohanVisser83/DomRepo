using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public class FixtureRequestDTO
    {
        public long SportId { get; set; }
        public long CommunityId { get; set; }
        public long SportsTypeId { get; set; }
    }
}
