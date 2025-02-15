using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.Home;
using Circular.Data.Repositories.Planners;
using Circular.Framework.Middleware.Emailer;
using Circular.Services.Email;

namespace Circular.Services.Master
{
    public class MasterService : IMasterService
    {
        private readonly IMasterRepository _masterRepository;
        private readonly IPlannerRepository _plannerRepository;
        private readonly IMailService _mailService;
        private string? connectionString;

        public MasterService(IMasterRepository masterRepository, IPlannerRepository plannerRepository, IMailService mailService)
        {
            _masterRepository = masterRepository;
            _plannerRepository = plannerRepository;
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

        public MasterService(string? connectionString)
        {
            this.connectionString = connectionString;
        }



        public async Task<IEnumerable<MasterEntity>?> GetAllAsync(string masterType, bool allRecords, long customerId)
        {
            return await _masterRepository.GetAllAsync(masterType, allRecords, customerId);

        }
        

      

        public async Task<int> RequestSupport(CustomerIssues customerIssues)
        {
            customerIssues.FillDefaultValues();
            var result = await _masterRepository.RequestSupport(customerIssues);
            var Emails = await _plannerRepository.SendEmailPlanner((long)customerIssues.CustomerId);
            var CommunityName = customerIssues.CommunityName;
            if (Emails != null)
            {

                MailRequest mailRequest = new MailRequest();
                mailRequest.FromUserId = customerIssues.CustomerId ?? 0;
                mailRequest.To = Emails[0].Email;
                mailRequest.ReferenceId = customerIssues.Id;
                MailSettings mailSettings = _mailService.EmailParameter(MailType.Support_Request, ref mailRequest);

                string body = mailRequest.Body;
                string[] PlaceHolders = { "$description", "$Mobile", "$Datetime", "$Customername", "$Community" , "$email" };
                string[] Values = { customerIssues.IssueDescription, customerIssues.Mobile, customerIssues.CreatedDate.ToString("dd MMM yyyy"), Emails[0].Name, CommunityName ,customerIssues.Name};
                if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
                {
                    for (int index = 0; index < PlaceHolders.Length; index++)
                        body = body.Replace(PlaceHolders[index], Values[index]);
                }
                mailRequest.Body = body;

                int result1 = Convert.ToInt16(await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings));
                return result1;

            }
            else
            {
                return 0;
            }
        }


        public async Task<bool> QRScan(QRScanRequest scanRequest)
        {
            try
            {
                return await _masterRepository.QRScan(scanRequest);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<int> SaveTimeZone(long id, string CurrentTimeZone)
        {
            return await _masterRepository.SaveTimeZone(id, CurrentTimeZone);
        }
        public async Task<IEnumerable<dynamic>> DeleteOTP()
        {
            return await _masterRepository.DeleteOTP();
        }
        public async Task<IEnumerable<dynamic>> UniqueDevices()
        {
            return await _masterRepository.UniqueDevices();
        }

        public async Task<IEnumerable<MasterEntity>?> GetCommunityHouseAllAsync(string masterType, long CommunityId)
        {
            return await _masterRepository.GetCommunityHouseAllAsync(masterType, CommunityId);

        }

        

        public async Task<IEnumerable<MasterEntity>?> GetCommunityClassesAllAsync(string masterType, long CommunityId)
        {
            return await _masterRepository.GetCommunityClassesAllAsync(masterType, CommunityId);

        }
    }
}
