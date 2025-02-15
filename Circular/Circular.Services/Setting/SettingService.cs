using Circular.Core.Entity;
using Circular.Data.Repositories.Setting;
using Circular.Data.Repositories.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Circular.Core.DTOs;
using Circular.Framework.Middleware.Emailer;
using Circular.Services.Email;
using System.Numerics;
using Org.BouncyCastle.Crypto;
using System.Reflection.Metadata.Ecma335;
using MailKit.BounceMail;
using Circular.Framework.Notifications;

namespace Circular.Services.Setting
{
    public class SettingService : ISettingService
    {
        IMailService _mailService;

        private readonly ISettingRepository _settingRepository;
        public SettingService(ISettingRepository settingrepository, IMailService mailService)
        {
            _settingRepository = settingrepository;
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }


        //user permission dropdown
        public async Task<List<communityroles>> GetRoles()
        {
            return await _settingRepository.GetRoles();
        }


        //sicknotes alert tab
        public async Task<List<Communities>> GetSickNotesAlert(long CommunityId)
        {
            return await _settingRepository.GetSickNotesAlert(CommunityId);
        }
        public long UpdateSickNotesAlert(long Id, string SickNoteRecipientName, string SickNotePhoneNumber, string SickNoteMailBox, string SickNoteEmail)
        {
            return _settingRepository.UpdateSickNotesAlert(Id, SickNoteRecipientName, SickNotePhoneNumber, SickNoteMailBox, SickNoteEmail);
        }


        //Add A new portal user
        public async Task<AddPermissionDTO> SaveSaveNewPortalUserAsync(AddPermissionDTO obj)
        {


            AddPermissionDTO x = await _settingRepository.SaveSaveNewPortalUserAsync(obj);
            MailRequest mailRequest = new MailRequest();
            mailRequest.FromUserId = obj.CustomerId;
            mailRequest.To = obj.customeremail;
            //mailRequest.ReferenceId =;
            MailSettings mailSettings = _mailService.EmailParameter(MailType.Explore, ref mailRequest);

            string body = mailRequest.Body;
            string[] PlaceHolders = { "$userfirstname" };
            string[] Values = { obj.customerFirstName };
            if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
            {
                for (int index = 0; index < PlaceHolders.Length; index++)
                    body = body.Replace(PlaceHolders[index], Values[index]);
            }
            mailRequest.Body = body;
            var mails = await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);

            return x;

            // return await _settingRepository.SaveSaveNewPortalUserAsync(Loggedinuser, customerFirstName, customeremail, customerLastName, customermobile, customerpassword, CommunityRoleId, IsOrganizer, CustomerId, AccessNumber, Communityid);
        }



        //Report IT alert tab
        public async Task<List<Communities>> GetReportItAlert(long CommunityId)
        {
            return await _settingRepository.GetReportItAlert(CommunityId);
        }
        public long UpdateReportItAlert(long Id, string RecipientNameReport, string EmailAddressReport, string PhoneNumberReport, long Reportcount)
        {
            return _settingRepository.UpdateReportItAlert(Id, RecipientNameReport, EmailAddressReport, PhoneNumberReport, Reportcount);
        }



        //Acess Control
        public async Task<int> DeleteAcessControlAsync(long Id)
        {
            return await _settingRepository.DeleteAcessControlAsync(Id);
        }
        public async Task<List<EventOrgBind>> GetAccessControlAsync(long Loggedinuser)
        {
            return await _settingRepository.GetAccessControlAsync(Loggedinuser);
        }
        //public async Task<List<EventOrgBind>> ViewUpdateAccessAsync(long Id)
        //{
        //    return await _settingRepository.ViewUpdateAccessAsync(Id);
        //}



        //audit
        public async Task<List<Core.Entity.Audit>> GetAuditByDateAsync(long Communityid, string STARTDATE)
        {
            return await _settingRepository.GetAuditByDateAsync(Communityid, STARTDATE);
        }


        //security checpoint   
        public Task<List<Communities>> GetSickNotesAlert()
        {
            throw new NotImplementedException();
        }

        public async Task<List<EventOrgBind>> GetViewPortalUser(long Id)
        {
            return await _settingRepository.GetViewPortalUser(Id);
        }
        

        //Feature Access
        public async Task<List<Features>> GetFeatureAccess()
        {
            return await _settingRepository.GetFeatureAccess();
        }

        //password reset
        public async Task<bool> GetPasswordReset(string Email)
        {

            MailRequest mailRequest = new MailRequest();
            mailRequest.To = Email;
            MailSettings mailSettings = _mailService.EmailParameter(MailType.Setting, ref mailRequest);
            string body = mailRequest.Body;

            string[] PlaceHolders = { "$FullName", "$urllink" };
            string[] Values = { "", "" };

            if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
            {
                for (int index = 0; index < PlaceHolders.Length; index++)
                    body = body.Replace(PlaceHolders[index], Values[index]);
            }
            mailRequest.Body = body;
            await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
            return true;

        }



        public long SaveResetPasswordAsync(long Loggedinuser, long PasswordActivationCode, bool IsVerified)
        {
            return _settingRepository.SaveResetPasswordAsync(Loggedinuser, PasswordActivationCode, IsVerified);
        }








        public async Task<int> SaveHouse(House data)
        {
            data.FillDefaultValues();
            return await _settingRepository.SaveHouse(data);
        }
        public async Task<List<House>> GetHouseList(long community)
        {
            return await _settingRepository.GetHouseList(community);
        }
        public async Task<List<House>> GetHouse(long Id)
        {
            return await _settingRepository.GetHouse(Id);
        }
        public async Task<int> DeleteHouse(long Id)
        {
            return await _settingRepository.DeleteHouse(Id);
        }
        public long UpdateHouse(long Id, string Name)
        {
            return _settingRepository.UpdateHouse(Id, Name);
        }

        public async Task<IEnumerable<CurrentCommunityPlan>> GetCommunitySubscriptionPlan(long community)
        {
            return await _settingRepository.GetCommunitySubscriptionPlan(community);
        }

        public async Task<IEnumerable<CommunityMemberTransaction>> GetCommunitySubsTransactionList(long community)
        {
            return await _settingRepository.GetCommunitySubsTransactionList(community);
        }

        public async Task<long> SaveCustomerStoreFrontAccess(List<CustomerStoreFrontAccess> customerStore)
        {
            return await _settingRepository.SaveCustomerStoreFrontAccess(customerStore);
        }

        public async Task<IEnumerable<CustomerStoreFrontAccess>> GetAccessStoreName(long customersId)
        {
            return await _settingRepository.GetAccessStoreName(customersId);
        }

        public async Task<long> SaveAdminFeatureAccess(List<AdminFeature> features, List<AdminFeature> uncheckedFeatures)
        {
            return await _settingRepository.SaveAdminFeatureAccess(features, uncheckedFeatures);
        }

        public async Task<IEnumerable<AdminFeature>> GetSelectedFeatures(long customersId)
        {
            return await _settingRepository.GetSelectedFeatures(customersId);

        }

        public async Task<int> RemoveSelectedStore(long Id, long storeId, long CustomerId)
        {
            return await _settingRepository.RemoveSelectedStore(Id, storeId, CustomerId);
        }

        public async Task<IEnumerable<SubscriptionDetails>> GetStripeCustomerSubscriptionId(long customerId, long CommunityId)
        {
            var customerEmail =  await _settingRepository.GetStripeCustomerSubscriptionId(customerId , CommunityId);

            if (customerEmail != null)
            {
                foreach (SubscriptionDetails cd in customerEmail)
                {
                    

                    MailRequest mailRequest = new MailRequest
                    {
                        FromUserId = cd.CustomerId,
                        ReferenceId = cd.CustomerId
                    };

                    MailSettings mailSettings = _mailService.EmailParameter(MailType.Cancle_Subscription_Commportal, ref mailRequest);
                    mailRequest.To = cd.Email;
                    mailRequest.Body = mailRequest.Body.Replace("$Email", cd.Email);
                    await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);

                }
            }

            return customerEmail;
        }

       
    }
}
