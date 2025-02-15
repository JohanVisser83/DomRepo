using Circular.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Services.Finance
{
    public interface IFinanceService
    {
        Task<long> NewPayment(TransactionRequest transactions, Customers currentCustomer);
        Task<long> SubscriptionPayment(TransactionRequest transactions);
        Task<string> CheckSubscriptionCustomer(long customerId);
        Task<long> SaveSubscriptionCustomer(long customerId, string StripeCustomerId);
        Task<List<SubscriptionFeaturesSelectedPlan>> GetSubscriptionSelectedFeatures(long Id);

        Task<dynamic> GetTransactions(long customerId);
        Task<dynamic> GetTransactionDetail(long transactionId, long customerId);
        public Task<int> SaveAsync(CustomerBankAccounts item);
        Task<int> DeleteAsync(long customerBankAccountId);
        Task<long> CustomerWithdrawalRequest(CustomerWithdrawalRequest customerWithdrawalRequest, Customers currentCustomer);
        Task<long> MakeWithdrawal(CustomerWithdrawalRequest Transaction);
        Task<bool> SendInvoice(long transactionId, long customerId, string ToEmailId, string WalletBalance);

        public Task<List<CustomerBankAccounts>> PostBankAccAsync();
        public Task<IEnumerable<dynamic>> GetBankDetailsAsync(long Id,long primaryId);
        public Task<List<Country>> GetMasterCountryAsync();
        public Task<List<Banks>> GetMasterBank(string Code);
        Task<IEnumerable<dynamic>> BankListWithdrawal(long BankId,long communityId);


        Task<dynamic> GetCommunityTransactions(DateTime startdate, DateTime enddate, long count, long primarycommunity);
        Task<IEnumerable<WithdrawalFeature>> GetWithdrawalFeature();
        public Task<long> RefundAsync(TransactionRequest transactions);
        Task<IEnumerable<dynamic>> GetRefundDetails(long CommunityId);
        Task<IEnumerable<dynamic>> GetEventName(string EventName, long CommunityId);
        Task<IEnumerable<dynamic>> GetAvailabelBalance(string EventName, long Id);
        Task<dynamic> PendingWithdrawal(long CommunityId, long customerId);
        Task<dynamic> PendingWithdrawalView(long TransactionTypeId, long Id);
        Task<IEnumerable<dynamic>> PaidWithdrawal(long transactionStatusId, long TransactionTypeId);

        public bool UpdateTransactionStatus(long transactionid, string gatewaystatus, string result);
        public string UpdateSubscriptionStatus(long transactionid, string gatewaystatus, string result);
        public string UpdateMemberSubscriptionStatus(long transactionid, string gatewaystatus, string result);
        public string UpdateCommunityMemberSubscriptionStatus(long transactionid, string gatewaystatus, string result);

        public string UpdateCommunityTransactionStatus(long transactionid, string gatewaystatus, string result);

        Task<int> DeleteBankAccount(long id);

        public Task<List<Banks>> GetUserBankListAsync(string Code, string Search);

        Task<Ewallet> WalletBalance(long CommunityId);
        public Task<long> SaveBillingAddress(SubscriptionBilling subscriptionBilling);
        Task<long> CreateSubscriptionUser(string email, string mobile, string firstName, string lastName);
        Task<int> SaveSubscriptionDetails(SubscriptionDetails newSubscription);
        public Transactions GetTransactionCustomerId(long TransactionId);

        Task<bool> GetCommunityExpiredSubscriptionDetails();
        Task<IEnumerable<HQCommunityTransactionDetails>> GetHQCommunityTransactions(long communityId);
    }
}
