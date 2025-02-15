namespace Circular.Core.DTOs
{
    public class GroupsDTO
    {
        public long Id { get; set; }
        public long? OrgId { get; set; }
        public string? GroupName { get; set; }
        public string? GroupDesc { get; set; }
        public long? CommunityID { get; set; }
        public bool? IsAddedByHQ { get; set; }

    }



}
