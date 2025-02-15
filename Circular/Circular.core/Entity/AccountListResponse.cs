using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.Entity
{
    public class AccountListResponse
    {
        public AccountListResponse()
        {
            this.AccountGroups = new List<AccountGroupResponse>();
        }



        public long? CustomerId { get; set; }
        public long? CollectionId { get; set; }
        public List<AccountGroupResponse> AccountGroups { get; set; }



    }
    public class AccountGroupResponse
    {
        public AccountGroupResponse()
        {
            this.AccountList = new List<CollectionAggregate>();
        }
        public DateTime? AccountDate { get; set; }
        public List<CollectionAggregate>? AccountList { get; set; }
    }

}
