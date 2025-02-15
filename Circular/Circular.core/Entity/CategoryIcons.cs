using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblCategoryIcons")]

public class CategoryIcons : BaseEntity
{
    public string ImagePath { get; set; }


    public override void ApplyKeys()
    {

    }
}

