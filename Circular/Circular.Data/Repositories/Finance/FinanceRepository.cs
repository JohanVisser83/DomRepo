using Circular.Core.Entity;
using Microsoft.Data.SqlClient;
using RepoDb;
using System.Data;
using System.Transactions;
using ZXing.QrCode.Internal;


namespace Circular.Data.Repositories.Finance
{
    public class FinanceRepository : DbRepository<SqlConnection>, IFinanceRepository
    {
        public FinanceRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<string> CheckSubscriptionCustomer(long customerId)
        {
            CustomerPaymentGatewayDetails customerPaymentGatewayDetails = QueryAsync<CustomerPaymentGatewayDetails?>(cpgd => cpgd.CustomerId == customerId
                                && cpgd.IsActive == true).Result.FirstOrDefault() ?? null;
            if(customerPaymentGatewayDetails != null)
            {
                return customerPaymentGatewayDetails.StripeCustomerId;
            }
            return "";
        }

        public async Task<long> SaveSubscriptionCustomer(long customerId, string StripeCustomerId)
        {
            try
            {
                CustomerPaymentGatewayDetails customerPaymentGatewayDetails = new CustomerPaymentGatewayDetails();
                customerPaymentGatewayDetails.CustomerId = customerId;
                customerPaymentGatewayDetails.StripeCustomerId = StripeCustomerId;
                customerPaymentGatewayDetails.FillDefaultValues();
                return await InsertAsync<CustomerPaymentGatewayDetails, int>(customerPaymentGatewayDetails);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<List<FeatureSubscriptionsFee>> GetSubscriptionFeatures(long customerId, long communityId)
        {
            return ExecuteQueryAsync<FeatureSubscriptionsFee>("Exec [dbo].[usp_GetCommunitySubscriptionFeatures] " + customerId + "," + communityId).Result.ToList();
        }

        public async Task<long> NewPayment(TransactionRequest transactions)
        {
            try
            {
				long result = (long)await ExecuteScalarAsync("Exec [dbo].[Usp_Finance_NewPayment] '"
				+ transactions.Amount + "','" + transactions.PaymentDesc + "','" + transactions.TransactionFrom + "','"
				+ transactions.MobileNumber + "','" + transactions.TransactionTypeId + "','"
				+ transactions.Currency + "','" + transactions.CommunityId + "','" + transactions.TransactionTo + "','" +
				transactions.TransactionStatusId + "','" + transactions.RequestedTransactionId + "','" + transactions.ServiceCharges + "','" + transactions.ReferenceType + "','" + transactions.ReferenceId +"'");
				return result;
			}
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<dynamic> GetTransactions(long customerId)
        {
            return await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Finance_Transactions] " + customerId);
        }
        public async Task<dynamic> GetTransactionDetail(long transactionId, long customerId)
        {
            return await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_Finance_TransactionDetails] " + transactionId + "," + customerId);
        }
        public async Task<int> SaveAsync(CustomerBankAccounts item)
        {
            try
            {
                return await InsertAsync<CustomerBankAccounts, int>(item);
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public async Task<int> DeleteAsync(long customerBankAccountId)
        {
            CustomerBankAccounts customerBankAccounts = QueryAsync<CustomerBankAccounts?>(cba => cba.Id == customerBankAccountId
            && cba.IsActive == true).Result.FirstOrDefault() ?? null;
            customerBankAccounts.ModifiedDate = DateTime.Now;
            customerBankAccounts.ModifiedBy = customerBankAccounts.CustomerId;
            customerBankAccounts.IsActive = false;
            var fields = Field.Parse<CustomerBankAccounts>(e => new
            {
                e.IsActive,
                e.ModifiedBy,
                e.ModifiedDate
            });
            var updatedRows = Update<CustomerBankAccounts>(entity: customerBankAccounts, fields: fields);
            return updatedRows;


        }
        public async Task<long> MakeWithdrawal(CustomerWithdrawalRequest transactions)
        {
            try
            {
                if (transactions.ServiceFee == null)
                    transactions.ServiceFee = 0;
                if (transactions.ReferenceType == null)
                    transactions.ReferenceType = "";
                if (transactions.ReferenceId == null)
                    transactions.ReferenceId = 0;

                string strCommand = "Exec [dbo].[USP_Finance_Withdrawal] '" + transactions.Amount + "','" + transactions.CustomerId + "','" + transactions.SavedBankId + "','" + transactions.ServiceFee + "','" + transactions.CommunityId + "','" + transactions.ReferenceType + "','" + transactions.ReferenceId +"','" +transactions.ReferenceComment+ "','"+ transactions.currency  + "'";

                return await ExecuteNonQueryAsync(strCommand);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }



        public async Task<List<CustomerBankAccounts>> PostBankAccAsync()
        {
            return QueryAll<CustomerBankAccounts>().Where(e => e.IsActive == true).ToList();
        }

        public async Task<IEnumerable<dynamic>> GetBankDetailsAsync(long Id, long primaryId)
        {
            return await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_GetSavedBankDetails] ' " + Id + "','" + primaryId + "'");

        }

        public async Task<List<Country>> GetMasterCountryAsync()
        {
            return QueryAll<Country>().Where(e => e.IsActive == true).ToList();
        }
        public async Task<List<Banks>> GetMasterBank(string Code)
        {
            return QueryAll<Banks>().Where(e => e.IsActive == true && e.Code == Code).ToList();
        }

        public async Task<List<Customers>> GetMobileNumAsync(string Mobile)
        {
            var result = await ExecuteQueryAsync<Customers>("Exec [dob].[USP_GetMobileNumeber]" + "" + Mobile + "");
            return (List<Customers>)result;
        }



        public async Task<dynamic> GetCommunityTransactions(DateTime startdate, DateTime enddate, long count, long primarycommunity)
        {
            try
            {
                var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Finance_CommunityTransactions] '" + startdate.ToString("yyyy-MM-dd") + "','" + enddate.ToString("yyyy-MM-dd") + "','" + count + "','" + primarycommunity + "'");
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<IEnumerable<WithdrawalFeature>> GetWithdrawalFeature()
        {
            return QueryAll<WithdrawalFeature>().Where(x => x.IsActive = true).ToList();
        }

        public async Task<long> RefundAsync(TransactionRequest transactions)
        {
            try
            {
                long result = await ExecuteNonQueryAsync("Exec [dbo].[Usp_finance_Refund] '"
                  + transactions.Amount + "','" + transactions.RefundNote + "','" + transactions.TransactionFrom + "','','"
                  + transactions.MobileNumber + "','" + transactions.TransactionTypeId + "','" + transactions.Currency + "','" + transactions.CommunityId + "','" + transactions.RefundId + "'");
                return result;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<IEnumerable<dynamic>> GetRefundDetails(long CommunityId)
        {
            return await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Finance_GetRefundDetails] '" + CommunityId + "'");
        }
        public async Task<IEnumerable<dynamic>> GetEventName(string EventName, long CommunityId)
        {
            return await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Finance_GetFeatureName] '" + EventName + "','0'," + CommunityId);


        }

        public async Task<IEnumerable<dynamic>> GetAvailabelBalance(string EventName, long Id)
        {
            return await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Finance_GetFeatureName] '" + EventName + "','" + Id + "'");
        }
        public async Task<dynamic> PendingWithdrawal(long CommunityId, long customerId)
        {
            return await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Finance_PendingWithdrawals] " + CommunityId + "," + customerId);
        }

        public bool UpdateTransactionStatus(long transactionid, string gatewaystatus, string result)
        {
            return ExecuteScalar<bool>("Exec [dbo].[Usp_Finance_UpdateTransactionStatus] " + transactionid + ",'" + gatewaystatus + "','" + result + "'");
        }

        public string UpdateSubscriptionStatus(long transactionid, string gatewaystatus, string result)
        {
            return ExecuteScalar<string>("Exec [dbo].[Usp_Finance_UpdateSubscriptionStatus] " + transactionid + ",'" + gatewaystatus + "','" + result + "'");
        }
        public string UpdateMemberSubscriptionStatus(long transactionid, string gatewaystatus, string result)
        {
            return ExecuteScalar<string>("Exec [dbo].[Usp_Finance_UpdateMemberSubscriptionStatus] " + transactionid + ",'" + gatewaystatus + "','" + result + "'");
        }
        public string UpdateCommunityMemberSubscriptionStatus(long transactionid, string gatewaystatus, string result)
        {
            return ExecuteScalar<string>("Exec [dbo].[Usp_Finance_UpdateMemberCommunitySubscriptionStatus] " + transactionid + ",'" + gatewaystatus + "','" + result + "'");
        }
        public string UpdateCommunityTransactionStatus(long transactionid, string gatewaystatus, string result)
        {
            return ExecuteScalar<string>("Exec [dbo].[Usp_Finance_UpdateCommunityTransactionStatus] " + transactionid + ",'" + gatewaystatus + "','" + result + "'");
        }
        public async Task<dynamic> PendingWithdrawalView(long TransactionTypeId, long Id)
        {
            try
            {
                return await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Finance_PendingView] " + TransactionTypeId + "," + Id + "");
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<IEnumerable<dynamic>> BankListWithdrawal(long BankId, long communityId)
        {
            try
            {
                return await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_BankListWithdrawal] '" +  communityId + "'," + BankId);
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public async Task<IEnumerable<dynamic>> PaidWithdrawal(long transactionStatusId, long TransactionTypeId)
        {
            return QueryAll<Transactions>().Where(e => e.TransactionStatusId == transactionStatusId && e.TransactionTypeId == TransactionTypeId && e.IsActive == true).ToList();
        }

        public async Task<int> DeleteBankAccount(long id)
        {
            CustomerBankAccounts delete = new CustomerBankAccounts();
            delete.Id = id;
            delete.IsActive = false;
            var fields = Field.Parse<CustomerBankAccounts>(x => new
            {
                x.IsActive


            });
            var updaterow = await UpdateAsync<CustomerBankAccounts>(entity: delete, fields: fields);
            return updaterow;
        }

        public async Task<List<Banks>> GetUserBankListAsync(string Code, string Search)
        {
            string query = "exec [USP_Finance_SearchBank]" + "" + Code + " ,'" + Search + "';";
            var result = await ExecuteQueryAsync<Banks>(query);
            return result.ToList<Banks>();
        }


        public async Task<Ewallet> WalletBalance(long CommunityId)
        {
            try
            {
                return  ExecuteQueryAsync<Ewallet>("Exec [dbo].[USP_Community_WalletBalance] " + CommunityId + "").Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public async Task<long> SaveBillingAddress(SubscriptionBilling subscriptionBilling)
        {
            try
            {
                return await InsertAsync<SubscriptionBilling, long>(subscriptionBilling);
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public async Task<List<SubscriptionFeaturesSelectedPlan>> GetSubscriptionSelectedFeatures(long Id)
        {
            return ExecuteQueryAsync<SubscriptionFeaturesSelectedPlan>("Exec [dbo].[usp_GetCommunitySelectedSubscriptionFeatures] " + Id.ToString()).Result.ToList();
        }

        public async Task<long> CreateSubscriptionUser(string email, string mobile, string firstName, string lastName)
        {
            try
            {
                long result = (long)await ExecuteScalarAsync("Exec [dbo].[Usp_Create_SubscriptionPortal_user] '"
                + email + "','" + mobile + "','" + firstName + "','" + lastName + "'");
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> SaveSubscriptionDetails(SubscriptionDetails newSubscription)
        {
            try
            {
                var key = 0;
                SubscriptionDetails subscription = QueryAsync<SubscriptionDetails?>(sd => sd.StripeSubscriptionId == newSubscription.StripeSubscriptionId
                && sd.CustomerId == newSubscription.CustomerId && sd.IsActive == true).Result.FirstOrDefault() ?? null;

                if(subscription != null)
                {
                    subscription.IsActive = false;
                    subscription.UpdateModifiedByAndDateTime();
                    var fields = Field.Parse<SubscriptionDetails>(x => new
                    {
                        x.IsActive,
                    });

                    key = await UpdateAsync<SubscriptionDetails>(entity: subscription, fields: fields);
                   
                }

                key = await InsertAsync<SubscriptionDetails, int>(newSubscription);
                return key;
               
            }
            catch(Exception ex)
            {
                return 0;
            }
           
        }

        public Transactions GetTransactionCustomerId(long transactionId)
        {
            var result = QueryAsync<Transactions?>(e => e.Id == transactionId && e.IsActive == true).Result.FirstOrDefault() ?? null;
            return result;

        }

        public async Task<ExpiredSubscriptionsNotifications> GetCommunityExpiredSubscriptionDetails()
        {
            ExpiredSubscriptionsNotifications expiredSubscriptionsNotifications = new ExpiredSubscriptionsNotifications();

            var  response = await  ExecuteQueryMultipleAsync("exec dbo.[USP_UpdateExpiredSubscriptions]");

            expiredSubscriptionsNotifications.actualExpiredSubscriptions = response.Extract<ActualExpiredSubscriptions>().ToList();
            expiredSubscriptionsNotifications.subscriptionDetails = response.Extract<SubscriptionDetails>().ToList();

            return expiredSubscriptionsNotifications;
        }

        public async Task<IEnumerable<HQCommunityTransactionDetails>> GetHQCommunityTransactions(long CommunityId)
        {
            try
            {
                var result = await ExecuteQueryAsync<HQCommunityTransactionDetails>("Exec [dbo].[Usp_HQ_CommunityTransactions] " + CommunityId);
                return result.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
