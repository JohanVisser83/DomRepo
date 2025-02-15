using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblCustomerBusinessIndex")]
public class CustomerBusinessIndex : BaseEntity
{
    public string? CompanyName { get; set; }
    public string? CompanyAddress { get; set; }
    public string? CompanyWebsite { get; set; }
    public string? CompanyEmail { get; set; }
    public string? ContactNumber { get; set; }
    public string? Description { get; set; }
    public long? OwnerId { get; set; }
    public string? CoverImage { get; set; }
    public string? CompanyLogo { get; set; }
    public long JobCount { get; set; }
    public string? Hours { get; set; }
    public long? CategoryId { get; set; }
    public string? Location { get; set; }
    public long CompanySize { get; set; }
    public long CommunityId { get; set; }
    public bool? IsBusinessapproved { get; set; }


    public override void ApplyKeys()
    {

    }
}
