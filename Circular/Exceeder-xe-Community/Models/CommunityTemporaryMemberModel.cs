using Circular.Core.Entity;

namespace Exceeder_xe_Community.Models
{
    public class CommunityTemporaryMemberModel
    {
        public CommunityTemporaryMemberModel()
        {
            this.communityTemporaryMember = new CommunityTemporaryMember();
        }

        public CommunityTemporaryMember communityTemporaryMember { get; set; }
    }
}
