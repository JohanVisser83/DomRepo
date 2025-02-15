using Circular.Core.Entity;
using Microsoft.Data.SqlClient;
using RepoDb;
using System.Drawing.Printing;


namespace Circular.Data.Repositories.Account
{
    public class AccountRepository : DbRepository<SqlConnection>, IAccountRepository
    {
        public AccountRepository(string connectionString) : base(connectionString)
        {
        }

        #region "Community Portal Functions"
        public async Task<IEnumerable<CollectionAggregate>> GetActiveAccount(long communityId)
        {
            string query = "exec [USP_Account_GetActiveAccount] " + communityId + ",0;";
            var result = await ExecuteQueryAsync<CollectionAggregate>(query);
            return (List<CollectionAggregate>)result;
        }
        public async Task<IEnumerable<CollectionAggregate>> GetClosedAccount(long communityId)
        {
            string query = "exec [USP_Account_GetActiveAccount] " + communityId + ",1;";
            var result = await ExecuteQueryAsync<CollectionAggregate>(query);
            return (List<CollectionAggregate>)result;
        }
        public async Task<int> DeleteActiveAccountitem(long id)
        {
            CollectionAggregate delete = new CollectionAggregate();
            delete.Id = id;
            delete.IsActive = false;
            delete.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<CollectionAggregate>(x => new
            {
                x.IsActive,x.ModifiedDate,x.ModifiedBy
            });
            var updaterow = await UpdateAsync<CollectionAggregate>(entity: delete, fields: fields);
            return updaterow;
        }
        public async Task<int> ClosedAccount(long id)
        {
            CollectionAggregate delete = new CollectionAggregate();
            delete.Id = id;
            delete.IsClosed = true;
            delete.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<CollectionAggregate>(x => new
            {
                x.IsClosed,
                x.ModifiedBy,
                x.ModifiedDate
            });
            var updaterow = await UpdateAsync<CollectionAggregate>(entity: delete, fields: fields);
            return updaterow;
        }
        public async Task<IEnumerable<dynamic>> GetCollectionData(long collectionId)
        {
            return await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_Account_GetActiveAccountData] '" + collectionId + "'");

        }
        public async Task<IEnumerable<dynamic>> GetViewAccountDetails(long Id)
        {
            try
            {
                return await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_Accounts_GetActiveAccountDataById] '" + Id + "'");
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<CustomerDetails>> GetIndividualDetails(long customerId)
        {
            var result = QueryAll<CustomerDetails>().Where(e => e.CustomerId == customerId && e.IsActive == true).ToList();
            return result;
        }
        public async Task<long> AddAccountDetails(CollectionAggregate addAccount)
        {
            try
            {
                CollectionAggregate newAccount =  ExecuteQueryAsync<CollectionAggregate>(
               "exec [dbo].[USP_Account_AddAccount]" + " '" + addAccount.CustomerId + "','" + addAccount.title + "','" + addAccount.CommunityId + "','"
               + addAccount.EventOrganiser + "','" +
               addAccount.GroupId + "','" + addAccount.Amount + "','" + addAccount.EventStartDate.ToString("MM-dd-yyyy HH:mm:ss") + "','" + addAccount.EventStartTime + "','"
               + addAccount.ExpirydateCollection.ToString("MM-dd-yyyy HH:mm:ss") + "','" + addAccount.Expirytimecollection + "','" 
               + addAccount.Scheduledeliverydate.ToString("MM-dd-yyyy HH:mm:ss") + "','" + addAccount.Scheduleddeliverytime + "','" 
               + addAccount.Description + "','" + addAccount.AccountMedia + "','" + addAccount.IsClosed + "','" + addAccount.Individual 
               + "' ,'" + addAccount.EventEndDate.ToString("MM-dd-yyyy HH:mm:ss") + "','" + addAccount.EventEndTime + "','" 
               + addAccount.TotalMemberAtCreation + "';").Result.FirstOrDefault();

                if (newAccount != null)
                    return newAccount.Id;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public async Task<List<CustomerDetails>> SendAddAccountEmail(long Id, int IsGroup)
        {
            var response = ExecuteQuery<CustomerDetails>("exec dbo.[USP_Account_SentAccountEmail] " + Id + "," + IsGroup).ToList();
            return response;
        }
        #endregion


        #region "API Functions"
        public async Task<List<CustomerDetails>> SendAccountPaidEmail(long customerId)
        {
            var result = QueryAll<CustomerDetails>().Where(c => c.CustomerId == customerId && c.IsActive == true).ToList();
            return result;
        }
        public long Pay(long CollectionReqId, long PayForUserId, decimal Amount, long LoggedInUserId, string Currency)
        {
            try
            {
                var response = ExecuteQuery<dynamic>("exec dbo.[Usp_Account_Pay] " + CollectionReqId + "," + PayForUserId + "," + Amount + "," + LoggedInUserId + "," + Currency).FirstOrDefault();
                return Convert.ToInt64(response.result ?? 0);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public AccountListResponse GetAccounts(long CustomerId, long CommunityId, int IsAllUpcomingOrPassed, long? Collectionid,int PageSize,int PageNumber)
        {
            List<CollectionAggregate>lstAccounts = ExecuteQuery<CollectionAggregate>("exec dbo.[Usp_Account_GetList] " + CustomerId + "," + CommunityId + "," + IsAllUpcomingOrPassed + "," + Collectionid + "," + PageSize + "," + PageNumber).ToList();
            AccountListResponse accountListResponse = null;
            if (lstAccounts != null)
            {
                accountListResponse = new AccountListResponse();
                lstAccounts.ForEach(account =>
                {
                    // group the data in dates
                    int index = accountListResponse.AccountGroups.FindIndex(ng => ng.AccountDate == (new DateTime(account.EventEndDate.Year, account.EventEndDate.Month, 1)));
                    if (index == -1)
                    {
                        AccountGroupResponse accountGroupResponse = new AccountGroupResponse();
                        accountGroupResponse.AccountDate = (new DateTime(account.EventEndDate.Year, account.EventEndDate.Month, 1));
                        accountGroupResponse.AccountList?.Add(account);
                        accountListResponse.AccountGroups.Add(accountGroupResponse);
                    }
                    else
                        accountListResponse.AccountGroups[index].AccountList?.Add(account);
                }
                );
            }
            return accountListResponse;
        }



        public async Task<int> SendBulkAccountsEmail(BulkAccountsEmail bulkAccountsEmail)
        {
                bulkAccountsEmail.FillDefaultValues();
                var key = await InsertAsync<BulkAccountsEmail, int>(bulkAccountsEmail);
                return key;
        }   

        public async Task<List<BulkAccountsEmail>> GetBulkEmaildetails()
        {
            List<BulkAccountsEmail> bulkAccountsEmail = ExecuteQuery<BulkAccountsEmail>("exec dbo.USP_Account_GetBulkAccountEmail").ToList();

            return bulkAccountsEmail;
        }
        public async Task<long> UpdateIsSentForBukEmail(BulkAccountsEmail bulkAccountsEmail)
        {
            bulkAccountsEmail.IsSent = true;
            bulkAccountsEmail.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<BulkAccountsEmail>(x => new
            {
                x.IsSent,
                x.ModifiedBy,
                x.ModifiedDate
            });
            return await UpdateAsync<BulkAccountsEmail>(entity: bulkAccountsEmail, fields: fields);
        }




        #endregion


    }
}
