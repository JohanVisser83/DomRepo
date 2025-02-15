using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblFundraiserProductImages")]


public  class FundraiserProductImages: BaseEntity
{

    
    public long FundraiserId { get; set; }  

    public string? ImagePath { get; set; }


    public override void ApplyKeys()
    {

    }

}

