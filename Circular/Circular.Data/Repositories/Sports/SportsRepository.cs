using Circular.Core.DTOs;
using Circular.Core.Entity;
using Microsoft.Data.SqlClient;
using RepoDb;
using System.Data;

namespace Circular.Data.Repositories.Sports
{
    public class SportsRepository : DbRepository<SqlConnection>, ISportsRepository
    {
          
        public SportsRepository(string connectionString) : base(connectionString)
        {
            
            
        }
        public SportsListResponse GetSportsAsync(long CommunityId, int UpcomingOrPast,bool IsGrouped)
        {
            List<Circular.Core.Entity.Sports> lstsports = ExecuteQuery<Circular.Core.Entity.Sports>("exec dbo.[Usp_Sports_List] "  +  CommunityId + "," + UpcomingOrPast).ToList();
            SportsListResponse sportsListResponse = null;
            if (lstsports != null)
            {
                sportsListResponse = new SportsListResponse();
                lstsports.ForEach(sport =>
                {
                    if (string.IsNullOrEmpty(sport.CoverImage))
                    {
                        Communities sponsorInformation = QueryAsync<Communities>(s => s.Id == CommunityId && s.IsActive == true).Result.LastOrDefault();
                        sport.CoverImage = string.IsNullOrEmpty(sponsorInformation?.OrgLogo) ? "default_logo.jpg" : sponsorInformation.OrgLogo;
                    }


                    SportsGroupResponse sportsGroupResponse = new SportsGroupResponse();
                    int index = 0;
                    // group the data in dates
                    if(IsGrouped)
                     index = sportsListResponse.SportsGroups.FindIndex(ng => ng.SportsStartDate == (new DateTime(sport.SportsDate.Year, sport.SportsDate.Month, 1)));
                    else
                        index = sportsListResponse.SportsGroups.FindIndex(ng => ng.SportsStartDate == (new DateTime(1900, 1, 1)));

                    if (index == -1)
                    {
                        if (IsGrouped)
                            sportsGroupResponse.SportsStartDate = (new DateTime(sport.SportsDate.Year, sport.SportsDate.Month, 1));
                        else
                            sportsGroupResponse.SportsStartDate = (new DateTime(1900, 1, 1));
                       
                        sportsGroupResponse.SportsList?.Add(sport);
                        sportsListResponse.SportsGroups.Add(sportsGroupResponse);
                    }
                    else
                    
                    sportsListResponse.SportsGroups[index].SportsList?.Add(sport);
                }
                );
            }
            return sportsListResponse;
        }
        public async Task<IEnumerable<dynamic>> GetSportsTypes()
        {
            var results = QueryAll<SportsType>().Where(e => e.IsActive == true);
            if (results != null)
                return results;
            else
                return null;
        }
        public async Task<IEnumerable<GetFixtures>?> GetSportsFixtures(long SportId, long CommunityId, long SportsTypeId)
		{
            IEnumerable<GetFixtures> result = await ExecuteQueryAsync<GetFixtures>("Exec [dbo].[Usp_Sports_Fixtures] " + SportId + "," + CommunityId + "," + SportsTypeId);
			return result;
		}
		public async Task<IEnumerable<dynamic>> GetTeamMemberAsync(long fixtureId)
        {
            var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Sports_Fixtures_GetTeamMembers] " + fixtureId);
            return result;
        }
  





        public async Task<List<SportsType>> GetActivity()
        {
            var results = QueryAll<SportsType>().Where(e => e.IsActive == true).OrderByDescending(e => e.Id).ToList();
            if (results != null)
                return results;
            else
                return null;
        }

        public async Task<bool> AddSports(string SportsName, string SportsDate, string SportPDF, long CommunityId, string CoverImage)
        {      
                String Query = "exec [dbo].[Usp_Sports_AddNewSports]" + " '" + SportsName + "','" + SportsDate + "','" + SportPDF + "','" + CommunityId + "','" + CoverImage + "'";
                int status = await ExecuteNonQueryAsync(Query);
                return true;          
        }

        public async Task<int> DeleteSportsAsync(long Id)
        {
			Core.Entity.Sports delete = new Core.Entity.Sports();
            delete.Id = Id;
            delete.IsActive = false;
            var fields = Field.Parse<Core.Entity.Sports>(x => (new
            {
                x.IsActive

            }));
            var updaterow = Update(entity: delete, fields: fields);
            return updaterow;
        }
        public async Task<bool> AddNewFixture(string FixtureTitle, DateTime Time, string Location, long SportId, string HomeTeam, string AwayTeam,long SportTypeId)
        {

            String Query = "exec [dbo].[Usp_Sports_AddFixture]" + " '" + FixtureTitle + "','" + Time + "','" + Location + "','" + SportId + "','" + HomeTeam + "','" + AwayTeam +"','" + SportTypeId+ "'";
            int status = await ExecuteNonQueryAsync(Query);
            return true;

        }
        public async Task<List<SportFixture>> GetUpcomingManageFixtureAsync(long SportId, long CommunityId, long SportsTypeId)
        {
            var results = await ExecuteQueryAsync<SportFixture>("exec [dbo].[Usp_Sports_Fixtures]" + SportId + "," + CommunityId + "," + SportsTypeId);
            if (results != null)
                return (List<SportFixture>)results;
            else
                return null;
        }
        public async Task<List<Core.Entity.Sports>> GetUpcomingAsync(long SportsTypeId,long CommunityId)
        {
            var results = await ExecuteQueryAsync<Core.Entity.Sports>("exec [dbo].[Usp_Sports_Upcoming]'" + SportsTypeId + "', '" + CommunityId + "'");
            if (results != null)
                return (List<Core.Entity.Sports>)results;
            else
                return null;
        }
            
        public async Task<List<Core.Entity.Sports>> GetUpcomingRecord()
        {
            var Message = QueryAll<Circular.Core.Entity.Sports>().Where(b => b.IsActive == true).ToList();
            return Message;
        }
        public async Task<List<Core.Entity.Sports>> GetResultAsync(long SportsTypeId, long CommunityId)
        {
            var results = await ExecuteQueryAsync<Core.Entity.Sports>("exec [dbo].[Usp_Sports_Result]'" + SportsTypeId + "', '" + CommunityId + "'");
            if (results != null)
                return (List<Core.Entity.Sports>)results;
            else
                return null;
        }
      

        public async Task<int> AddTeamMember(SportsTeamMember teamMember,long Community)
        {
            try
            {
                var key = await InsertAsync<SportsTeamMember, int>(teamMember);
                return key;
            }
            catch (Exception ex)
            {
                return 0;

            }

        }
        public async Task<int> DeleteTeamMembersAsync(long Id)
        {
            SportsTeamMember delete = new SportsTeamMember();
            delete.Id = Id;
            delete.IsActive = false;
            var fields = Field.Parse<SportsTeamMember>(x => new
            {
                x.IsActive

            });
            var updaterow = Update<SportsTeamMember>(entity: delete, fields: fields);
            return updaterow;
        }
        public async Task<List<SportsTeamMember>> GetAddedTeamMemberAsync(long TeamId)
        {
            var results = await ExecuteQueryAsync<SportsTeamMember>("exec [dbo].[Usp_Sports_GetHomeTeamMember]" + " '" + TeamId + "'");
            if (results != null)
                return (List<SportsTeamMember>)results;
            else
                return null;

        }

        public async Task<int> SaveResult(long Id, string Result)
        {
            SportFixture update = new SportFixture();
            update.Id = Id;
            update.Result = Result;
            var fields = Field.Parse<SportFixture>(x => new
            {
            
                x.Result

            });
            var updaterow = Update<SportFixture>(entity: update, fields: fields);
            return updaterow;
        }
        public async Task<int> DeleteSportsType(long Id)
        {
            Core.Entity.SportsType delete = new Core.Entity.SportsType();
            delete.Id = Id;
            delete.IsActive = false;
            var fields = Field.Parse<Core.Entity.SportsType>(x => (new
            {
                x.IsActive

            }));
            var updaterow = Update(entity: delete, fields: fields);
            return updaterow;
        }
        public long UpdateSportType(long Id, string Activities)
        {
            SportsType updatetype = new SportsType();
            updatetype.Id = Id;
            updatetype.Activities = Activities;
            updatetype.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<SportsType>(x => new
            {
                x.Activities,
                x.ModifiedBy,
                x.ModifiedDate
            });
            var updatedRows = Update<SportsType>(entity: updatetype, fields: fields);
            return updatedRows;   
        }
        public async Task<int> SaveSportType(SportsType data)
        {
            var key = await InsertAsync<SportsType, int>(data);
            return key;
        }
        public async Task<List<SportsType>> GetSportsType(long Id)
        {
            var results = QueryAll<SportsType>().Where(e => e.Id == Id && e.IsActive == true).ToList();
            if (results != null)
                return results;
            else
                return null;
        }


    }
}
