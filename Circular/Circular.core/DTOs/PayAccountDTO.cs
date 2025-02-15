using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public  class PayAccountDTO
    {
        public long CollectionReqId { get; set; }
        public long LoggedInUserId { get; set; }
        public decimal Amount { get; set; }
        public long PayForUserId { get; set; }
        public string Currency { get; set; }
    }
}
