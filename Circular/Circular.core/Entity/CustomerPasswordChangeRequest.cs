using RepoDb.Attributes;
using System;
namespace Circular.Core.Entity;

[Map("tblCustomerPasswordChangeRequest")]

public class CustomerPasswordChangeRequest : BaseEntity
{
        public long? CustomerId { get; set; }
        public long?PasswordActivationCode { get; set; }
        public bool?IsVerified { get; set; }

        public override void ApplyKeys()
        {

        }
}

