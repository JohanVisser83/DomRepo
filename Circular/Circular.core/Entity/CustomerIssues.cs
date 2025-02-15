using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCustomerIssues")]

public class CustomerIssues : BaseEntity
{
    public long? CustomerId { get; set; }
    public string? Name { get; set; }
    
    public string?  Mobile { get; set; }
  
    public string? Attachment { get; set; }
    public string? DeviceName { get; set; }
    public string? IssueDescription { get; set; }

    public string? CommunityName { get; set; }


    public override void ApplyKeys()
    {

    }
}