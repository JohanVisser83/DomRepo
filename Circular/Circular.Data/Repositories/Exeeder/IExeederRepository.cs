using System;
using Circular.Core.Entity;

namespace Circular.Data.Repositories.Exeeder
{
    public interface IExeederRepository
    {
        Task<int> SaveCommunityTempDetails(CommunityTemporaryMember communityTemporary);
    }
}
