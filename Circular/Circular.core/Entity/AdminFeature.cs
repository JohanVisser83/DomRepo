using RepoDb.Attributes;
namespace Circular.Core.Entity;

[Map("tblAdminFeature")]
public class AdminFeature : BaseEntity
{
    public long CommunityId { get; set; }   

    public long CustomerId { get; set; }    
    public long FeatureId { get; set; }    

    public string Code { get; set; }
    
    public string FeatureName { get; set; } 

    public override void ApplyKeys()
    {

    }
}

