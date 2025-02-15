using Circular.Core.Entity;


namespace Circular.Services.Account
{
    public interface IAccountServices
    {


        #region "Community Portal Functions"
        
        Task<IEnumerable<CollectionAggregate>> GetActiveAccount(long CommunityId);
        Task<IEnumerable<CollectionAggregate>> GetClosedAccount(long CommunityId);
        Task<int> DeleteActiveAccountitem(long id);
        Task<int> ClosedAccount(long id);
        Task<bool> SendBulkEmails();

        Task<IEnumerable<dynamic>> GetCollectionData(long collectionId);

        Task<IEnumerable<dynamic>> GetViewAccountDetails(long Id);

        Task<CustomerDetails> GetIndividualDetails(long customerId);
        Task<long> AddAccountDetails(Core.Entity.CollectionAggregate account);

        //Task<List<CustomerDetails>> SendAddAccountEmail(long communityId);

        //Task<List<CustomerDetails>> SendAddAccountEmailToGroup(long groupID);
        #endregion


        #region "API Functions"
        public Task<long> Pay(long CollectionReqId, long PayForUserId, decimal Amount, long LoggedInUserId, string Currency);
        public AccountListResponse GetAccounts(long CustomerId, long CommunityId, int IsAllUpcomingOrPassed, long? Collectionid,int pageSize,int pageNumber);
        Task<List<CustomerDetails>> SendAccountPaidEmail(long customerId);
        #endregion

    }

}