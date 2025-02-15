namespace Circular.Core.Entity
{
    public abstract class MasterEntity : BaseEntity
    {
        public string Name { get; set; }
        public string? Desc { get; set; }
        public string? Code { get; set; }
    }
}
