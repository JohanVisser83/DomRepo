using Circular.Core.Entity;
using Circular.Services.Community;

namespace CircularWeb.Models
{
    public class BaseModel
    {
        public bool IsOwner { get; set; }
        //public bool SubscriptionStatus { get; set; }

        public string CommunityLogo { get; set; }
        public List<Features> CommunityFeatures { get; set; }
        public List<CommunityRestrictedFeatures> CommunityRestFeatures { get; set; }

        public string SubscriptionStatus { get; set; }  

        public bool IsFeatureAvailable(string featureCode) 
        { 
            bool IsFeatureAvailable = true;
            if (SubscriptionStatus == "active")
            {
                if (CommunityFeatures != null)
                {
                    List<Features> features = CommunityFeatures.Where(f => f.code == featureCode).ToList();
                    if (features == null || features.Count <= 0)
                        IsFeatureAvailable = false;
                }
                else
                    IsFeatureAvailable = false;
            }
            else
                IsFeatureAvailable = false;
            return IsFeatureAvailable; 
        }


        


    }
}
