using Circular.Core.DTOs;
using Circular.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Services.Setting
{
    public interface ISettingService
    {
        //user permission dropdown
        public Task<List<communityroles>> GetRoles();


        //sicknotes alert tab
        public Task<List<Communities>> GetSickNotesAlert();


        //sicknotes alert tab
        public Task<List<Communities>> GetSickNotesAlert(long CommunityId);
        long UpdateSickNotesAlert(long Id, string SickNoteRecipientName, string SickNotePhoneNumber, string SickNoteMailBox, string SickNoteEmail);


        //Add A new portal user
        Task<AddPermissionDTO> SaveSaveNewPortalUserAsync(AddPermissionDTO obj);


        //Report iT alert tab
        public Task<List<Communities>> GetReportItAlert(long CommunityId);
        long UpdateReportItAlert(long Id, string RecipientNameReport, string EmailAddressReport, string PhoneNumberReport, long Reportcount);



        //Acess Control
        Task<int> DeleteAcessControlAsync(long Id);
        public Task<List<EventOrgBind>> GetAccessControlAsync(long Loggedinuser);



        //audit
        public Task<List<Core.Entity.Audit>> GetAuditByDateAsync(long CommunityId, string STARTDATE);


        //Security Checkpoint       
        public Task<List<EventOrgBind>> GetViewPortalUser(long Id);        

        //Featur Access
        public Task<List<Features>> GetFeatureAccess();

        //password reset
        Task<bool> GetPasswordReset(string Email);
        long SaveResetPasswordAsync(long Loggedinuser, long PasswordActivationCode, bool IsVerified);
        public Task<int> SaveHouse(House data);
        public Task<List<House>> GetHouseList(long community);
        public Task<List<House>> GetHouse(long Id);
        Task<int> DeleteHouse(long Id);
        long UpdateHouse(long Id, string Name);
        Task<IEnumerable<CurrentCommunityPlan>> GetCommunitySubscriptionPlan(long community);
        Task<IEnumerable<CommunityMemberTransaction>> GetCommunitySubsTransactionList(long community);
        Task<long> SaveCustomerStoreFrontAccess(List<CustomerStoreFrontAccess> customerStore);
        Task<IEnumerable<CustomerStoreFrontAccess>> GetAccessStoreName(long customersId);
        Task<long> SaveAdminFeatureAccess(List<AdminFeature> features,  List<AdminFeature> uncheckedFeatures);
        Task<IEnumerable<AdminFeature>> GetSelectedFeatures(long customersId);
        Task<int> RemoveSelectedStore(long Id, long storeId, long CustomerId);
        Task<IEnumerable<SubscriptionDetails>> GetStripeCustomerSubscriptionId(long customerId, long CommunityId);
    }

}
