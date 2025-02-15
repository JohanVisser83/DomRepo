using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.CreateCommunity;
using Circular.Data.Repositories.Message;
using Circular.Data.Repositories.Sports;
using Circular.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Services.Sports
{
    public  class SportsService : ISportsService
    {
        private readonly ISportsRepository _SportsRepository;



        public SportsService(ISportsRepository SportsRepository)
        {
            _SportsRepository = SportsRepository;
            
        }

        public SportsListResponse GetSportsAsync(long CommunityId, int UpcomingOrPast,bool IsGrouped)
        {
            return  _SportsRepository.GetSportsAsync(CommunityId, UpcomingOrPast,IsGrouped);
        }

        public async Task<IEnumerable<dynamic>> GetSportsTypes()
        {
            return await _SportsRepository.GetSportsTypes();
        }

		public async Task<IEnumerable<GetFixtures>> GetSportsFixtures(long SportId, long CommunityId, long SportsTypeId)
		{
			return await _SportsRepository.GetSportsFixtures(SportId, CommunityId, SportsTypeId);
		}
		public async Task<IEnumerable<dynamic>> GetTeamMemberAsync(long fixtureId)
        {
            return await _SportsRepository.GetTeamMemberAsync(fixtureId);
        }

      





        public async Task<List<SportsType>> GetActivity()
        {
            return await _SportsRepository.GetActivity();
        }
        public async Task<bool> AddSports(string SportsName, string SportsDate, string SportPDF, long CommunityId, string CoverImage)
        {
            return await _SportsRepository.AddSports(SportsName, SportsDate, SportPDF, CommunityId, CoverImage);
        }
        public async Task<int> DeleteSportsAsync(long Id)
        {
            return await _SportsRepository.DeleteSportsAsync(Id);
        }
        public async Task<bool> AddNewFixture(string FixtureTitle, DateTime Time, string Location, long SportId, string HomeTeam, string AwayTeam,long SportTypeId)
        {
            return await _SportsRepository.AddNewFixture(FixtureTitle, Time, Location, SportId, HomeTeam, AwayTeam, SportTypeId);
        }
        public async Task<List<SportFixture>> GetUpcomingManageFixtureAsync(long SportId, long CommunityId, long SportsTypeId)
        {
            return await _SportsRepository.GetUpcomingManageFixtureAsync( SportId,  CommunityId,  SportsTypeId);
        }   
        public async Task<List<Core.Entity.Sports>> GetUpcomingRecord()
        {
            return await _SportsRepository.GetUpcomingRecord();
        }       
        public async Task<int> AddTeamMember(SportsTeamMember teamMember,long Community)
        {
            teamMember.FillDefaultValues();
            return await _SportsRepository.AddTeamMember(teamMember, Community);
        }
        public async Task<int> DeleteTeamMembersAsync(long Id)
        {
            return await _SportsRepository.DeleteTeamMembersAsync(Id);
        }
        public async Task<List<SportsTeamMember>> GetAddedTeamMemberAsync(long TeamId)
        {
            return await _SportsRepository.GetAddedTeamMemberAsync(TeamId);
        }
        public async Task<int> SaveResult(long Id, string Result)
        {
            return await _SportsRepository.SaveResult(Id,  Result);
        }
        public async Task<int> DeleteSportsType(long Id)
        {
            return await _SportsRepository.DeleteSportsType(Id);
        }
        public long UpdateSportType(long Id, string Activities)
        {
            return  _SportsRepository.UpdateSportType(Id, Activities);
        }
        public async Task<int> SaveSportType(SportsType data)
        {
            data.FillDefaultValues();
            return await _SportsRepository.SaveSportType(data);
        }
        public async Task<List<SportsType>> GetSportsType(long Id)
        {
            return await _SportsRepository.GetSportsType(Id);
        }

    }
}
