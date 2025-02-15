
namespace Circular.Core.DTOs
{
    public class EventOrgBindDTO 
    {
        public int id { get; set; }
        public long CustomerId { get; set; }
        public long CommunityRoleId { get;set; }
        public long communityId { get; set; }
        public bool? lsOrganizer { get; set; }



        public string FullName { get; set; }
        public string Email { get; set; }
        public string AccessNumber { get; set; }
        public string Mobile { get; set; }
        public string RolesName { get; set; }
        //public long Id { get; set; }
        //public long CustomerId { get; set; }
        //public long RolesId { get; set; }




    }
}
