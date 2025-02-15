using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("mtblCommunityCategory")]

    public class CommunityCategory : BaseEntity
    {
        public string Category { get; set; }

        public override void ApplyKeys()
        {

        }
    }

