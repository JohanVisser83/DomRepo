using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Services.Master
{
    public interface IMasterService
    {
        public Task<IEnumerable<MasterEntity>?> GetAllAsync(string masterType, bool allRecords,long customerId);
        Task<int> RequestSupport(CustomerIssues customerIssues);
		Task<bool> QRScan(QRScanRequest scanRequest);
        Task<int> SaveTimeZone(long id, string CurrentTimeZone);
        Task<IEnumerable<dynamic>> DeleteOTP();
        Task<IEnumerable<dynamic>> UniqueDevices();
        public Task<IEnumerable<MasterEntity>?> GetCommunityHouseAllAsync(string masterType, long communityId);

        public Task<IEnumerable<MasterEntity>?> GetCommunityClassesAllAsync(string masterType, long communityId);

        
    }
}
