using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblOrganizationsEmail")]

public class OrganizationsEmail : BaseEntity
{
    public string? OrgName { get; set; }
    public string? Slogan { get; set; }
    public string? OrgLegalName { get; set; }
    public string? Website { get; set; }
    public string? Description { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? Logo { get; set; }



    public override void ApplyKeys()
    {

    }
}
