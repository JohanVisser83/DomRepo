using Circular.Core.DTOs;
using Circular.Core.Entity;
using Tweetinvi.Models;

namespace Circular.Services.Sports
{
    public interface ISportsService
    {

        public SportsListResponse GetSportsAsync(long CommunityId, int UpcomingOrPast, bool IsGrouped);
        Task<IEnumerable<dynamic>> GetSportsTypes();
		Task<IEnumerable<GetFixtures>> GetSportsFixtures(long SportId, long CommunityId, long SportsTypeId);
		Task<IEnumerable<dynamic>> GetTeamMemberAsync( long fixtureId);




       



        
        public Task<List<SportsType>> GetActivity();
        Task<bool> AddSports(string SportsName, string SportsDate, string SportPDF, long CommunityId, string CoverImage);
        Task<int> DeleteSportsAsync(long Id);
        Task<bool> AddNewFixture(string FixtureTitle, DateTime Time, string Location, long SportId, string HomeTeam, string AwayTeam,long SportTypeId);
        public Task<List<SportFixture>> GetUpcomingManageFixtureAsync(long SportId, long CommunityId, long SportsTypeId);   
        Task<List<Core.Entity.Sports>> GetUpcomingRecord(); 
        public Task<int> AddTeamMember(SportsTeamMember teamMember,long Community);
        Task<int> DeleteTeamMembersAsync(long Id);
        public Task<List<SportsTeamMember>> GetAddedTeamMemberAsync(long TeamId);
        Task<int> SaveResult(long Id ,string Result);
        Task<int> DeleteSportsType(long Id);
        long UpdateSportType(long Id,string Activities);
        public Task<int> SaveSportType(SportsType data);
        public Task<List<SportsType>> GetSportsType(long Id);
    }
}
