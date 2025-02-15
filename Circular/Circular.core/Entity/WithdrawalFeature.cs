
using RepoDb.Attributes;
namespace Circular.Core.Entity
{
    [Map("tblWithdrawalFeaturetab")]
    public class WithdrawalFeature : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public override void ApplyKeys()
        {

        }
    }
}

