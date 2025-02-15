using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.DTOs
{
    public class PostAmountCompaign
    {
        public long FundraiserTypeId { get; set; }
        public string FundraiserTitle { get; set; }
        public long OrganizerId { get; set; }
        public decimal ProductAmount { get; set; }
        public string PDFLink { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
