using Circular.Core.Entity;
using Microsoft.Extensions.Options;

namespace Circular.Core.DTOs
{
    public class AddPermissionDTO
    {

        public AddPermissionDTO()
        {
            if (CustomerStores == null)
                CustomerStores = new List<CustomerStoreFrontAccess>();

            if(adminFeatures == null)
                adminFeatures = new List<AdminFeature>();   

        }

        public long CustomerId { get; set; }
        public string? customerFirstName { get; set; }
        public string? customerLastName { get; set; }
        public string? customeremail { get; set; }
        public string? customermobile { get; set; }
        public string? customerpassword { get; set; }
        public long CommunityRoleId { get; set; }
        public long IsOrganizer { get; set; }
        public string? AccessNumber { get; set; }
        public long Communityid { get; set; }

        public List<CustomerStoreFrontAccess> CustomerStores { get; set; }

        public List<AdminFeature> adminFeatures { get; set; }
        public List<AdminFeature> uncheckedadminFeatures { get; set; }

    }
}
