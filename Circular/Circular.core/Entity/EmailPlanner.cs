using RepoDb.Attributes;

namespace Circular.Core.Entity
{
    [Map("tblPlanner")]
    public class EmailPlanner:BaseEntity
    {
        public long PlannerId { set; get; }
        public string? Email { set; get; }
        public override void ApplyKeys()
        {
            
        }
    }
}
