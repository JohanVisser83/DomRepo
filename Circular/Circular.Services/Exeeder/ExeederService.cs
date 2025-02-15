using Circular.Core.Entity;
using Circular.Data.Repositories.CreateCommunity;
using Circular.Data.Repositories.Exeeder;

namespace Circular.Services.Exeeder
{
    public  class ExeederService : IExeederService
    {
        private readonly IExeederRepository _ExeederRepository;
        public ExeederService(IExeederRepository exeederRepository)
        {
            _ExeederRepository = exeederRepository;
        }


        public async Task<int> SaveCommunityTempDetails(CommunityTemporaryMember communityTemporary)
        {
            communityTemporary.FillDefaultValues();
            return await _ExeederRepository.SaveCommunityTempDetails(communityTemporary);
        }
    }
}
