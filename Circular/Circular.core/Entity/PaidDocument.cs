using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblPaidDocuments")]

    public class PaidDocument : BaseEntity
    {
        public long UserId { get; set; }
        public long DocumentId { get; set; }

        public override void ApplyKeys()
    {

    }
}
