using Circular.Core.Entity;

namespace CircularHQ.Models
{
    public class AffiliateManagementModel : BaseModel
    {
        public AffiliateManagementModel()
        {
            this.AffiliateCode = new List<AffiliateCode>();
            this.lstaffiliateCode = new List<AffiliatedCodeDetails>(); 
        }
        public IEnumerable<AffiliateCode> AffiliateCode { get; set; }
        public IEnumerable<AffiliatedCodeDetails> lstaffiliateCode { get; set; }   
    }
}
