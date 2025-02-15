using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblBanks")]
public class Banks : MasterEntity
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? BankDescription { get; set; }
    public string? Images { get; set; }


    public override void ApplyKeys()
    {

    }
}