using Circular.Core.Entity;

namespace Circular.Services.Exeeder
{
    public interface IExeederService
    {
        public Task<int>  SaveCommunityTempDetails(CommunityTemporaryMember communitySignUp);
    }
}
