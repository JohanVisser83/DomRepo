using Circular.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public  class FundraiserCollectionDTO : BaseEntityDTO
    {
       
            public long FundraiserId { get; set; }

            public long UserId { get; set; }

            public decimal Amount { get; set; }

            public bool IsCollected { get; set; }

            public long TransactionId { get; set; }


          
        
    }
}
