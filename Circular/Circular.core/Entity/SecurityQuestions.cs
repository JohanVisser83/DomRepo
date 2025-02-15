using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblSecurityQuestions")]

public class SecurityQuestions : BaseEntity
{
    public string Name { get; set; }


    public override void ApplyKeys()
    {

    }
}
