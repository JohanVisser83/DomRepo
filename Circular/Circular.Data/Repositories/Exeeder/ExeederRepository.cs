using Circular.Core.Entity;
using RepoDb;
using Microsoft.Data.SqlClient;


namespace Circular.Data.Repositories.Exeeder
{
    public  class ExeederRepository : DbRepository<SqlConnection>, IExeederRepository
    {
        public ExeederRepository(string connectionString) : base(connectionString)
        {

        }



        public async Task<int> SaveCommunityTempDetails(CommunityTemporaryMember communityTemporaryMember)
        {
            try
            {
                var result = await InsertAsync<CommunityTemporaryMember, int>(communityTemporaryMember);
                return result;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
    }
}
