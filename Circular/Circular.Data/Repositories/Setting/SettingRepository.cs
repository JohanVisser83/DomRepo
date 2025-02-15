using Circular.Core.DTOs;
using Circular.Core.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Crypto;
using RepoDb;
using System.Reflection;
using System.Xml.Linq;

namespace Circular.Data.Repositories.Setting
{
    public class SettingRepository : DbRepository<SqlConnection>, ISettingRepository
    {
        public SettingRepository(string connectionString) : base(connectionString)
        {

        }

        //user permission dropdown
        public async Task<List<communityroles>> GetRoles()
        {
            var results = QueryAll<communityroles>().Where(e => e.IsActive == true).ToList();
            if (results != null)
                return results;
            else
                return null;
        }


        //sicknotes alert tab
        public async Task<List<Communities>> GetSickNotesAlert(long CommunityId)
        {
            var results = await ExecuteQueryAsync<Communities>("exec [dbo].[Usp_Setting_GetSickNoteSettings]" + " '" + CommunityId + "'");
            if (results != null)
                return (List<Communities>)results;
            else
                return null;
        }
        public long UpdateSickNotesAlert(long Id, string SickNoteRecipientName, string SickNotePhoneNumber, string SickNoteMailBox, string SickNoteEmail)
        {
            Communities community = new Communities();
            community.Id = Id;
            community.SickNoteRecipientName = SickNoteRecipientName;
            community.SickNotePhoneNumber = SickNotePhoneNumber;
            community.SickNoteMailBox = SickNoteMailBox;
            community.SickNoteEmail = SickNoteEmail;
            var fields = Field.Parse<Communities>(x => new
            {
                x.SickNoteRecipientName,
                x.SickNotePhoneNumber,
                x.SickNoteMailBox,
                x.SickNoteEmail,
            });
            var updaterow = Update<Communities>(entity: community, fields: fields);
            return updaterow;
        }


        //Add A new portal user       
        public async Task<AddPermissionDTO> SaveSaveNewPortalUserAsync(AddPermissionDTO obj)
        {
            try
            {
                // delete 
                String Query = "exec [dbo].[Usp_Setting_UpdateAccessControlView]" + obj.CustomerId + "";
                int status = await ExecuteNonQueryAsync(Query);

                //String Query = "exec [dbo].[Usp_Setting_PortalUser] '" + Loggedinuser + "','" + customerFirstName + "','" + customeremail + "','" + customerLastName + "','" + customermobile + "','" + customerpassword + "','" + CommunityRoleId + "','" + IsOrganizer + "','" + Loggedinuser + "','" + AccessNumber + "','" + communityid + "'";
                var response = await ExecuteQueryAsync<AddPermissionDTO>("exec [dbo].[Usp_Setting_PortalUser] '" + obj.CustomerId + "','" + obj.customerFirstName + "','" + obj.customeremail + "','" + obj.customerLastName + "','" + obj.customermobile + "','" + obj.customerpassword + "','" + obj.CommunityRoleId + "','" + obj.IsOrganizer + "','" + obj.CustomerId + "','" + obj.AccessNumber + "','" + obj.Communityid + "'");

                //var Customerid = await ExecuteQueryAsync(Query);                
                return response.FirstOrDefault<AddPermissionDTO>();

                //return 1;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        //acces control
        public async Task<int> DeleteAcessControlAsync(long Id)
        {
            EventOrgBind accessdelete = new EventOrgBind();
            accessdelete.Id = Id;
            accessdelete.IsActive = false;
            var fields = Field.Parse<EventOrgBind>(x => new
            {
                x.IsActive

            });
            var updaterow = Update<EventOrgBind>(entity: accessdelete, fields: fields);
            return updaterow;
        }

        public async Task<List<EventOrgBind>> GetAccessControlAsync(long Loggedinuser)
        {
            var results = await ExecuteQueryAsync<EventOrgBind>("exec [dbo].[Usp_Setting_GetUserAccessProfileHQ]" + " '" + Loggedinuser + "'");

            if (results != null)
                return (List<EventOrgBind>)results;
            else
                return null;
        }





        //ReportIT alert tab
        public async Task<List<Communities>> GetReportItAlert(long CommunityId)
        {
            var results = await ExecuteQueryAsync<Communities>("exec [dbo].[Usp_Setting_GetReportIT]" + " '" + CommunityId + "'");

            if (results != null)
                return (List<Communities>)results;
            else
                return null;
        }

        public long UpdateReportItAlert(long Id, string RecipientNameReport, string EmailAddressReport, string PhoneNumberReport, long Reportcount)
        {
            Communities VarReport = new Communities();
            VarReport.Id = Id;
            VarReport.RecipientNameReport = RecipientNameReport;
            VarReport.EmailAddressReport = EmailAddressReport;
            VarReport.PhoneNumberReport = PhoneNumberReport;

            //VarReport.Reportcount = Reportcount;
            //  varsicknotealerts.IsActive = true;
            var fields = Field.Parse<Communities>(x => new
            {
                x.RecipientNameReport,
                x.EmailAddressReport,
                x.PhoneNumberReport,

                //x.Reportcount


            });
            var updaterow = Update<Communities>(entity: VarReport, fields: fields);
            return updaterow;
        }


        //audit
        public async Task<List<Core.Entity.Audit>> GetAuditByDateAsync(long CommunityId, string STARTDATE)
        {
            var results = await ExecuteQueryAsync<Core.Entity.Audit>("exec [dbo].[USP_Setting_GetAuditDataByDate]" + " '" + CommunityId + "','" + STARTDATE + "'");
            if (results != null)
                return (List<Core.Entity.Audit>)results;
            else
                return null;
        }


        //security checkpoint 
        public async Task<List<EventOrgBind>> GetViewPortalUser(long Id)
        {
            var results = await ExecuteQueryAsync<EventOrgBind>("exec [dbo].[Usp_Setting_GetAccessControlView]" + " '" + Id + "'");

            if (results != null)
                return (List<EventOrgBind>)results;
            else
                return null;
        }

        //feature access
        public async Task<List<Features>> GetFeatureAccess()
        {

            var results = await ExecuteQueryAsync<Features>("exec [dbo].[Usp_Setting_GetFeatures]");

            if (results != null)
                return (List<Features>)results;
            else
                return null;

        }

        //password reset
        public long SaveResetPasswordAsync(long Loggedinuser, long PasswordActivationCode, bool IsVerified)
        {
            CustomerPasswordChangeRequest OBJ = new CustomerPasswordChangeRequest();
            OBJ.CustomerId = Loggedinuser;
            OBJ.PasswordActivationCode = PasswordActivationCode;
            OBJ.IsVerified = IsVerified;

            var fields = Field.Parse<CustomerPasswordChangeRequest>(x => new
            {
                x.CustomerId,
                x.PasswordActivationCode,
                x.IsVerified


            });
            var updaterow = Update<CustomerPasswordChangeRequest>(entity: OBJ, fields: fields);
            return updaterow;
        }








        public async Task<int> SaveHouse(House data)
        {
            var key = await InsertAsync<House, int>(data);
            return key;
        }
        public async Task<List<House>> GetHouseList(long community)
        {
            var results = QueryAll<House>().Where(e => e.CommunityId == community && e.IsActive == true).OrderByDescending(e => e.Id).ToList();
            if (results != null)
                return results;
            else
                return null;
        }
        public async Task<List<House>> GetHouse(long Id)
        {
            var results = QueryAll<House>().Where(e => e.Id == Id && e.IsActive == true).ToList();
            if (results != null)
                return results;
            else
                return null;
        }
        public async Task<int> DeleteHouse(long Id)
        {
            Core.Entity.House delete = new Core.Entity.House();
            delete.Id = Id;
            delete.IsActive = false;
            var fields = Field.Parse<Core.Entity.House>(x => (new
            {
                x.IsActive

            }));
            var updaterow = Update(entity: delete, fields: fields);
            return updaterow;
        }
        public long UpdateHouse(long Id, string Name)
        {
            House updatehouse = new House();
            updatehouse.Id = Id;
            updatehouse.Name = Name;
            updatehouse.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<House>(x => new
            {
                x.Name,
                x.ModifiedBy,
                x.ModifiedDate
            });
            var updatedRows = Update<House>(entity: updatehouse, fields: fields);
            return updatedRows;
        }

        public async Task<IEnumerable<CurrentCommunityPlan>> GetCommunitySubscriptionPlan(long community)
        {
            var result = await ExecuteQueryAsync<CurrentCommunityPlan>("Exec [dbo].[Usp_Community_CurrentMembershipPlan]" + "" + community + "");
            return (IEnumerable<CurrentCommunityPlan>)result;
        }

        public async Task<IEnumerable<CommunityMemberTransaction>> GetCommunitySubsTransactionList(long community)
        {
            var result = await ExecuteQueryAsync<CommunityMemberTransaction>("Exec [dbo].[Usp_Setting_CommunitySubscriptionTransactions]" + "" + community + "");
            return (IEnumerable<CommunityMemberTransaction>)result;
        }

        public async Task<long> SaveCustomerStoreFrontAccess(List<CustomerStoreFrontAccess> customerStore)
        {
            long key = 0;
            foreach (var customer in customerStore)
            {
                customer.FillDefaultValues();
                key = (long)await InsertAsync<CustomerStoreFrontAccess, long>(customer);
            }
            return key;
        }

        public async Task<IEnumerable<CustomerStoreFrontAccess>> GetAccessStoreName(long customersId)
        {
            var results = QueryAll<CustomerStoreFrontAccess>().Where(e => e.CustomerId == customersId && e.IsActive == true).ToList();
            return results;
        }


        public async Task<long> SaveAdminFeatureAccess(List<AdminFeature> checkedFeatures, List<AdminFeature> uncheckedFeatures)
        {
            long lastInsertedKey = 0;

            // Handle Checked Features 
            foreach (var feature in checkedFeatures)
            {
                feature.FillDefaultValues();
                lastInsertedKey = (long)await InsertAsync<AdminFeature, long>(feature);
            }

            //// Handle Unchecked features 
            //foreach (var feature in uncheckedFeatures)
            //{
            //    if (feature.Id > 0)
            //    {
            //        await DeleteAsync<AdminFeature>(feature.Id);
            //    }
            //}

            return lastInsertedKey;
        }


        public async Task<IEnumerable<AdminFeature>> GetSelectedFeatures(long customersId)
        {
            var results = QueryAll<AdminFeature>().Where(e => e.CustomerId == customersId && e.IsActive == true).ToList();
            return results;
        }

        public async Task<int> RemoveSelectedStore(long Id, long storeId, long CustomerId)
        {
            CustomerStoreFrontAccess customerStoreFrontAccess = new CustomerStoreFrontAccess();
            customerStoreFrontAccess.Id = Id;
            customerStoreFrontAccess.StoreId = storeId;
            customerStoreFrontAccess.CustomerId = CustomerId;
            customerStoreFrontAccess.IsActive = false;
            //customerStoreFrontAccess.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<CustomerStoreFrontAccess>(x => new
            {
                x.IsActive,

            });
            var updaterow = Update<CustomerStoreFrontAccess>(entity: customerStoreFrontAccess, fields: fields);
            return updaterow;
        }

        public async Task<IEnumerable<SubscriptionDetails>> GetStripeCustomerSubscriptionId(long customerId, long CommunityId)
        {
            try
            {
                IEnumerable<SubscriptionDetails> results = ExecuteQueryAsync<SubscriptionDetails>("Exec [dbo].[USP_CommunityCancelSubscription]" + "" + CommunityId + "").Result.ToList<SubscriptionDetails>();
                if (results.Count() > 0)
                {
                    //cancel the customer community subscription

                    foreach (SubscriptionDetails oSubscriptionDetails in results)
                    {
                        oSubscriptionDetails.SubscriptionStatus = "canceled";
                        oSubscriptionDetails.ModifiedBy = customerId;
                        oSubscriptionDetails.UpdateModifiedByAndDateTime();
                    }
                    var Qfields = Field.Parse<SubscriptionDetails>(e => new
                    {
                        e.Id
                    });
                    var fields = Field.Parse<SubscriptionDetails>(x => new
                    {
                        x.Id,
                        x.SubscriptionStatus,
                        x.ModifiedBy,
                        x.ModifiedDate
                    });
                    int updaterow = await UpdateAllAsync<SubscriptionDetails>(entities: results, qualifiers: Qfields, fields: fields);



                    //disable the customer community
                    var communitylist = QueryAll<CustomerCommunity>().Where(e => e.CommunityId == CommunityId && e.IsActive == true && e.CustomerId != customerId).ToList();
                    foreach (var community in communitylist)
                    {
                        community.IsActive = false;
                        community.ModifiedBy = customerId;
                        community.UpdateModifiedByAndDateTime();
                    }
                    var Quafields = Field.Parse<CustomerCommunity>(e => new
                    {
                        e.Id
                    });
                    var fieldsToUpdate = Field.Parse<CustomerCommunity>(x => new
                    {
                        x.Id,
                        x.IsActive,
                        x.ModifiedBy,
                        x.ModifiedDate
                    });
                    int updaterow1 = await UpdateAllAsync<CustomerCommunity>(entities: communitylist, qualifiers: Quafields, fields: fieldsToUpdate);


                    //disable to community
                    var fieldsToUpdateinCommunity = Field.Parse<Communities>(x => new
                    {
                        x.Id,
                        x.IsDeletedByAdmin,
                        x.ModifiedBy,
                        x.ModifiedDate
                    });
                    Communities ocommunities = new Communities() { ModifiedBy = customerId, ModifiedDate = DateTime.Now, IsDeletedByAdmin = true, Id = CommunityId };
                    var updaterow2 = Update<Communities>(entity: ocommunities, fields: fieldsToUpdateinCommunity);


                }
                return results;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }


    }
}
