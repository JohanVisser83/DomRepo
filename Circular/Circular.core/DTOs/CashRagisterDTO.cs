using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public  class CashRagisterDTO
    {
        public long Count { get; set; }
        public decimal TotalSales { get; set; }

        public decimal CashAmount { get; set; }

        public decimal TotalRecipts { get; set; }

        public decimal Diffrence {get; set; }
    }
}
