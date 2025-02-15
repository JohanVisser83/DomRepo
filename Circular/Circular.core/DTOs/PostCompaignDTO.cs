
using Circular.Core.Entity;

namespace Circular.Core.DTOs
{
    public class PostCompaignDTO
    {


        public long FundraiserTypeId { get; set; }
        public string FundraiserTitle { get; set; }
        public long OrganizerId { get; set; }
        public decimal ProductAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string PDFLink { get; set; }
        public string Description { get; set; }
        public string FormHyperlink { get; set; }
        public string ImagePath { get; set; }
       

    }


       

}


