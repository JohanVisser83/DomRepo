using RepoDb.Attributes;
using System;

namespace Circular.Core.Entity;

[Map("tblAudit")]

public class Audit : BaseEntity
{
    public long? CustomerId { get; set; }    
    public long? LinkedCustomerId { get; set; }
    public long? Communityid { get; set; }
    public long? TransactionId { get; set; }
    public string? Activity { get; set; }
    public string? ActivityDesc { get; set; }
    public long? InterfaceTypeId { get; set; }
    public string? Device { get; set; }
    public string? ClientDetail { get; set; }



    public string UserName { get; set; }
    public string AuditMessage { get; set; }
    public string AuditTime { get; set; }

    public override void ApplyKeys()
    {

    }
}
