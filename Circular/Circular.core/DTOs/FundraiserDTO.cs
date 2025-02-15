using Circular.Core.Entity;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace Circular.Core.DTOs
{
    public class FundraiserDTO: BaseEntityDTO
    {
     
        public FundraiserDTO()
        {
            if (Images == null)
                Images = new List<FundraiserProductImagesDTO>();
        }  
        public long CommunityId { get; set; }
        public long FundraiserTypeId { get; set; }
        public string? Title { get; set; }
        public long OrganizerId { get; set; }
        public decimal Amount { get; set; }
        public decimal? CollectedAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public IFormFile pdf { get; set; }
        public string? PDFLink { get; set; }
        public string Description { get; set; }
        public string? FormLink { get; set; }
        public int? DaysLeft { get; set; }
        public string? TypeOfFundraiser { get; set; }
        public string? Organizername { get; set; }
        public string? Mobile { get; set; }
        public string? OrganizerProfile { get; set; }
        public string ImagePath { get; set; }
        public List<FundraiserProductImagesDTO> Images { get; set; }



        public string FundraiserTitle { get; set; }
        public decimal ProductAmount { get; set; }
        public string FormHyperlink { get; set; }
    }
}
