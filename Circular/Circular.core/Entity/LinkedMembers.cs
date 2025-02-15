using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblCustomerLinkedMembers")]

public class LinkedMembers : BaseEntity
{
    public long FromCustId { get; set; }
    public long ToCustId { get; set; }
    public long LinkingStatusId { get; set; }


    public override void ApplyKeys()
    {

    }
    public long LinkedMemberId { get; set; }
    public long RequestToFirstName { get; set; }
    public long RequestToLastName { get; set;}
}
