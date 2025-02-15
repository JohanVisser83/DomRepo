using Microsoft.EntityFrameworkCore;
using RepoDb.Attributes;
namespace Circular.Core.Entity;

[Map("tblUserCommunityRole")]
public class EventOrgBind : BaseEntity
{
    
    public long CustomerId { get; set; }
    public long CommunityRoleId { get; set; }
    public long communityId { get; set; }
    public int? lsOrganizer { get; set; }
    public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Email { get; set; }
	public string RolesName { get; set; }
	public string AccessNumber { get; set; }
    public string Mobile { get; set; }   
    public long Id { get; set; }
 


    public override void ApplyKeys()
    {

    }
}

public class ActiveAccount 
{
    public string title { get; set; }
    public long? CommunityId { get; set; }
    public long? EventOrganiser { get; set; }
    public long? Invite { get; set; }
    public long? Individual { get; set; }
    public long? GroupId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }

}

