using Circular.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Data.Repositories.Account
{
    public interface IAccountRepository
    {

        #region "Community Portal Functions"

        Task<IEnumerable<CollectionAggregate>> GetActiveAccount(long communityId);
        Task<IEnumerable<CollectionAggregate>> GetClosedAccount(long communityId);
        Task<int> ClosedAccount(long id);
        Task<int> DeleteActiveAccountitem(long id);
        Task<IEnumerable<dynamic>> GetCollectionData(long collectionId);
        Task<IEnumerable<dynamic>> GetViewAccountDetails(long Id);
        Task<long> AddAccountDetails(CollectionAggregate addAccount);

        Task<List<CustomerDetails>> GetIndividualDetails(long customerId);
        Task<List<CustomerDetails>> SendAddAccountEmail(long Id, int IsGroup);


        Task<List<BulkAccountsEmail?>> GetBulkEmaildetails();
        Task<long> UpdateIsSentForBukEmail(BulkAccountsEmail bulkAccountsEmail);
        #endregion


        #region "API Functions"
        public long Pay(long CollectionReqId, long PayForUserId,decimal Amount,long LoggedInUserId,string Currency);
        public AccountListResponse GetAccounts(long CustomerId, long CommunityId, int IsAllUpcomingOrPassed, long? Collectionid,int PageSize,int PageNumber);
        Task<List<CustomerDetails>> SendAccountPaidEmail(long customerid);
        Task<int> SendBulkAccountsEmail(BulkAccountsEmail bulkAccountsEmail);
       
       
        #endregion


    }
}
