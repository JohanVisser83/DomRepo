using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public class PayFundraiserDTO
    {
        public long FundraiserTypeId { get; set; }
        public long LoggedInUserId { get; set; }
        public decimal Amount { get; set; }
        public long PayForUserId { get; set; }
        public string Currency { get; set; }
    }



    public class FundhubActiveOrderDTO
    {
        public long? loggedinuser { get; set; }
        public long? CommunityId { get; set; }
        public bool IsCollected { get; set; }

    }
}