using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblFundraiser")]
public class Fundraiser : BaseEntity
  {

    public Fundraiser() 
    {
        if (Images == null)
            Images = new List<FundraiserProductImages>();
    }

        public long Id { get; set; }
        public long CommunityId { get; set; }
        public long FundraiserTypeId { get; set; }
        public string? Title { get; set; }
        public long OrganizerId { get; set; }   
        public decimal? Amount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? PDFLink { get; set; }
        public string? Description { get; set; }
        public string? FormLink { get; set; }
        public string? TypeOfFundraiser { get; set; }

        public string? OrganizerName { get; set; }

        public string? OrganizerMobile { get; set; }

        public string? OrganizerProfile { get; set; }
        public string? ImagePath { get; set; }

        public decimal? CollectedAmount { get; set; }

        public int? DaysLeft { get; set; }

        public bool IsArchive { get; set; }
    public List<FundraiserProductImages> Images { get; set; }

      

    public override void ApplyKeys()
       {

       }
 }

public class FundListResponse
{
    public FundListResponse()
    {
        this.FundGroups = new List<FundGroupResponse>();
    }

    public List<FundGroupResponse> FundGroups { get; set; }
}


public class FundGroupResponse
{
    public FundGroupResponse()
    {
        this.FundraiserList = new List<Fundraiser>();
    }
    public DateTime? Date { get; set; }
    public List<Fundraiser>? FundraiserList { get; set; }
}
