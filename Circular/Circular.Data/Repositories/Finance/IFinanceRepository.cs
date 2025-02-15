using Circular.Core.Entity;

namespace Circular.Data.Repositories.Finance
{
    public interface IFinanceRepository
    {
        Task<string> CheckSubscriptionCustomer(long customerId);
        Task<long> SaveSubscriptionCustomer(long customerId, string StripeCustomerId);
        Task<List<FeatureSubscriptionsFee>> GetSubscriptionFeatures(long customerId, long communityId);

        Task<long> NewPayment(TransactionRequest transactions);
        Task<dynamic> GetTransactions(long customerId);
        Task<dynamic> GetTransactionDetail(long transactionId, long customerId);
        Task<int> SaveAsync(CustomerBankAccounts item);
        Task<int> DeleteAsync(long customerBankAccountId);
        Task<long> MakeWithdrawal(CustomerWithdrawalRequest transactions);


        Task<List<CustomerBankAccounts>> PostBankAccAsync();
        public Task<IEnumerable<dynamic>> GetBankDetailsAsync(long Id, long primaryId);
        Task<List<Country>> GetMasterCountryAsync();
        public Task<List<Banks>> GetMasterBank(string Code);
        Task<List<Customers>> GetMobileNumAsync(string Mobile);
        Task<dynamic> GetCommunityTransactions(DateTime startdate, DateTime enddate, long count, long primarycommunity);
        Task<IEnumerable<WithdrawalFeature>> GetWithdrawalFeature();
        Task<long> RefundAsync(TransactionRequest transactions);
        Task<IEnumerable<dynamic>> GetRefundDetails(long CommunityId);
        Task<IEnumerable<dynamic>> GetEventName(string EventName, long CommunityId);
        Task<IEnumerable<dynamic>> GetAvailabelBalance(string EventName, long Id);
        Task<dynamic> PendingWithdrawal(long CommunityId, long customerId);
        Task<IEnumerable<dynamic>> BankListWithdrawal(long BankId, long communityId);
        public bool UpdateTransactionStatus(long transactionid, string gatewaystatus, string result);
        public string UpdateSubscriptionStatus(long transactionid, string gatewaystatus, string result);
        public string UpdateMemberSubscriptionStatus(long transactionid, string gatewaystatus, string result);
        public string UpdateCommunityMemberSubscriptionStatus(long transactionid, string gatewaystatus, string result);
        public string UpdateCommunityTransactionStatus(long transactionid, string gatewaystatus, string result);

        
        Task<dynamic> PendingWithdrawalView(long TransactionTypeId, long Id);
        Task<IEnumerable<dynamic>> PaidWithdrawal(long transactionStatusId, long TransactionTypeId);
        Task<int> DeleteBankAccount(long id);
        public Task<List<Banks>> GetUserBankListAsync(string Code, string Search);
        Task<Ewallet> WalletBalance(long CommunityId);
        Task<long> SaveBillingAddress(SubscriptionBilling subscriptionBilling);
        Task<List<SubscriptionFeaturesSelectedPlan>> GetSubscriptionSelectedFeatures(long Id);
        Task<long> CreateSubscriptionUser(string email, string mobile, string firstName, string lastName);
        Task<int> SaveSubscriptionDetails(SubscriptionDetails newSubscription);
        public Transactions GetTransactionCustomerId(long transactionId);
        Task<ExpiredSubscriptionsNotifications> GetCommunityExpiredSubscriptionDetails();
        Task<IEnumerable<HQCommunityTransactionDetails>> GetHQCommunityTransactions(long CommunityId);
    }
}
