using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblPermission")]
public class MasterPermission : BaseEntity
    {
        public string Name { get; set; }    
        public string Description { get; set; }
        public override void ApplyKeys()
        {

        }
    }

