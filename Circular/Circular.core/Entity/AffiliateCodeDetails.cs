using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblAffiliate")]

    public class AffiliatedCodeDetails : BaseEntity
    {

       
        public long AffiliateCodeId { get; set; }
        public string FirstName { get; set; }  
        public string LastName { get; set; }
        public string Email { get; set; }
        public string phone { get; set; }

        public string UsageCount { get; set; }
        public string CommunityName { get; set; }

        public string AffiliateCode { get; set; }   



        public override void ApplyKeys()
        {

        }
    }
