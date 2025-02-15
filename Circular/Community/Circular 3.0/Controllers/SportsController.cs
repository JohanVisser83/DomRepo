using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.User;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.Sports;
using CircularWeb.Business;
using CircularWeb.filters;
using CircularWeb.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using MailKit.Outlook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nancy.Diagnostics.Modules;
using Stripe;
using System.Security.Claims;

namespace CircularWeb.Controllers
{
    [Authorize]
    public class SportsController : Controller
    {
        private readonly ISportsService _SportsService;
        private readonly ICommunityService _communityService;

        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IGlobal _global;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SportsModel sportsModel = new SportsModel();
        List<string> uploadedpaths = new List<string>();
        private string OIDCUrl;




        public SportsController(ISportsService SportsService, IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration configuration
            ,  IHelper helper, IHttpContextAccessor httpContextAccessor, ICustomerRepository customerRepository, ICommunityService communityService)
        {

            _SportsService = SportsService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _global = new Global(_httpContextAccessor, _config,customerRepository);
            _communityService = communityService;
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);

        }

        [HttpGet]
        [ActionLog("Sports", "{0} opened sports.")]
        public async Task<IActionResult> Sports()
        {
            CurrentUser currentUser = _global.GetCurrentUser();
            long Loggedinuser = currentUser.Id;
            long Community = currentUser.PrimaryCommunityId;
            sportsModel.lstactivity = await _SportsService.GetActivity();
            sportsModel.lstsportsupcoming =  _SportsService.GetSportsAsync(Community, 1 ,  false);
            sportsModel.CommunityLogo = currentUser.CustomerInfo.PrimaryCommunity.CommunityLogo;
            sportsModel.CommunityFeatures = _communityService.Features(Community,Loggedinuser).Result.ToList();
            sportsModel.IsOwner = currentUser.CustomerInfo.PrimaryCommunity.OwnerCustomerId == Loggedinuser ? true : false;
            sportsModel.SubscriptionStatus = currentUser.SubscriptionStatus;
            if (!sportsModel.IsFeatureAvailable("SP-007"))
                throw new ArgumentNullException("Unauthroized : You dont have permission to access this functionality.");
            else

                return View(sportsModel);
        }

       
        [HttpPost]
        [ActionLog("Sports", "{0} saved new sport.")]
        public async Task<ActionResult> AddNewSports(SportsDTO obj)
        {
            
                try
                {
                    string SportsDate = obj.SportsDate.ToString("MM-dd-yyyy HH:mm:ss");
                    string uploadpdffile = string.Empty, CoverImage = string.Empty;
                    if (Request.Form.Files.Count > 0)
                    {
                        var file = Request.Form.Files[0];
                        string UploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString();
                        SavemultipleFilesupdate(file, UploadFolder, this.Request);
                        var data = uploadedpaths;

                        if (data.Count() > 0)
                            uploadpdffile = data[0];
                        if (data.Count() > 1)
                            CoverImage = data[1];
                    }
                    obj.SportPDF = Convert.ToString(uploadpdffile);
                    obj.CoverImage = Convert.ToString(CoverImage);

                    long Community = _global.currentUser.PrimaryCommunityId;
                    Sports sports = _mapper.Map<Sports>(obj);
                    obj.CommunityId = Community;
                    var result1 = await _SportsService.AddSports(obj.SportsName, SportsDate, obj.SportPDF, obj.CommunityId, obj.CoverImage);
                    obj = _mapper.Map<SportsDTO>(sports);
                    if (result1 != null)
                        return Json(new { success = true, message = "Data Save Successfully!" });
                    else
                        return Json(new { success = false, message = "Data  not Save Successfully!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while Adding New Sports" });
                }
            



            

        }

        public void SavemultipleFilesupdate(IFormFile file, string uploadFolder, HttpRequest request)
        {
            var path = "";
            var returnfilepath = "";
            for (int i = 0; i < request.Form.Files.Count; i++)
            {
                file = request.Form.Files[i];
                string url = $"{request.Scheme}://{request.Host}{request.PathBase}" + uploadFolder;
                string filesPath = Directory.GetCurrentDirectory() + uploadFolder;
                if (!Directory.Exists(filesPath))
                    Directory.CreateDirectory(filesPath);

                var uniqueFileName = GetUniqueFileNameupdate(file.FileName);
                //string fileName = Path.GetFileName(uniqueFileName);

                path = Path.Combine(filesPath, uniqueFileName);
                file.CopyToAsync(new FileStream(path, FileMode.Create));
                returnfilepath = Path.Combine(url, uniqueFileName);
                uploadedpaths.Add(returnfilepath);
            }

        }
        private string GetUniqueFileNameupdate(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

        [HttpPost]
        [ActionLog("Sports", "{0} delete sports.")]
        public async Task<ActionResult> DeleteSports(long Id)
        {
            var delete = await _SportsService.DeleteSportsAsync(Id);
            if (delete > 0)
                return Json(new { success = true, message = "Deleted Successfully!" });
            else
                return Json(new { success = false, message = "Not Successfully!" });
        }
        [ActionLog("Sports", "{0} fetched upcoming sports.")]
        public async Task<IActionResult> GetUpcoming(UpcomingSportsDTO obj)
        { 
            long Community = _global.currentUser.PrimaryCommunityId;
            obj.CommunityId = Community;
            var lstdata =  _SportsService.GetSportsAsync(Community,1,false);
            return Json(new { success = true, Data = lstdata.SportsGroups[0].SportsList });
        }

        [ActionLog("Sports", "{0} fetched result sports.")]
        public async Task<IActionResult> GetResult(UpcomingSportsDTO obj)
        {
            long communityId = _global.currentUser.PrimaryCommunityId;
            obj.CommunityId = communityId;
            var lstresult = _SportsService.GetSportsAsync(communityId, 0, false);
            return Json(new { success = true, Data = lstresult.SportsGroups[0].SportsList });
        }

        [HttpPost]
        [ActionLog("Sports", "{0} saved fixture.")]
        public async Task<ActionResult> AddFixture(ManageFixtureDTO obj)
        {
            var result = await _SportsService.AddNewFixture(obj.FixtureTitle, obj.Time, obj.Location, obj.SportId,obj.HomeTeam,obj.AwayTeam,obj.SportTypeId);
            if (result != null)
                return Json(new { success = true, message = "Data Updated Successfully!" });

            else
                return Json(new { success = false, message = "Data Not Updated Successfully!" });
        }

        [ActionLog("Sports", "{0} fetched upcoming fixture.")]
        public async Task<IActionResult> GetUpcomingManageFixture(long SportId, long CommunityId, long SportsTypeId)
        {
            long communityId = _global.currentUser.PrimaryCommunityId;
            var data = await _SportsService.GetUpcomingManageFixtureAsync(SportId, communityId, SportsTypeId);
            return Json(new { success = true, Data = data });
        }
      
        [HttpPost]
        [ActionLog("Sports", "{0} saved team member inside fixture.")]
        public async Task<ActionResult> SaveTeamMember(SportsTeamMemberDTO obj)
        {

            long Community = _global.currentUser.PrimaryCommunityId;
            obj.CommunityId = Community;
            SportsTeamMember TeamMembers = _mapper.Map<SportsTeamMember>(obj);
            var result1 = await _SportsService.AddTeamMember(TeamMembers, Community);

            obj = _mapper.Map<SportsTeamMemberDTO>(TeamMembers); 
            if (result1 > 0)
                return Json(new { success = true, message = "Data Save Successfully" });
            else
                return Json(new { success = false, message = "Data  not Save Successfully!" });

        }
        [HttpPost]
        [ActionLog("Sports", "{0} deleted team members inside manage fixture.")]
        public async Task<ActionResult> DeleteTeamMembers(long Id)
        {
            var Member = await _SportsService.DeleteTeamMembersAsync(Id);
            if (Member > 0)
                return Json(new { success = true, message = "Deleted Successfully!" });
            else
                return Json(new { success = false, message = "Not Successfully!" });
        }

        [ActionLog("Sports", "{0} fetched team members inside fixture.")]
        public async Task<IActionResult> GetAddedTeamMember(long TeamId)
        {
            var tresult = await _SportsService.GetAddedTeamMemberAsync(TeamId);
            return Json(new { success = true, Data = tresult });
        }

        [HttpPost]
        [ActionLog("Sports", "{0} saved final score result.")]
        public async Task<ActionResult> SaveResult(long Id, string Result)
        {            
            var SR = await _SportsService.SaveResult(Id, Result);
            if (SR > 0)
                return Json(new { success = true, message = "Data Updated Successfully!" });
            else
                return Json(new { success = false, message = "Data Not Updated Successfully!" });
        }

        [HttpPost]
        [ActionLog("Sports", "{0} delete sports Type.")]
        public async Task<ActionResult> DeleteSportsType(long Id)
        {
            var delete = await _SportsService.DeleteSportsType(Id);
            if (delete > 0)
                return Json(new { success = true, message = "Deleted Successfully!" });
            else
                return Json(new { success = false, message = "Not Successfully!" });
        }

        [HttpPost]
        [ActionLog("SportsType", "{0} updated Sport Type.")]
        public async Task<ActionResult> UpdateSportType(SportsTypeDTO obj)
        {
            long result = _SportsService.UpdateSportType(obj.Id, obj.Activities);
            if (result != null)
                return Json(new { success = true, message = "Data Updated Successfully!" });

            else
                return Json(new { success = false, message = "Data Not Updated Successfully!" });
        }
        [HttpPost]
        [ActionLog("Sports", "{0} Add a Sports Type.")]
        public async Task<ActionResult> SaveSportType([FromBody] SportsTypeDTO obj)
        {
            SportsType sporttype = _mapper.Map<SportsType>(obj);
            var result1 = await _SportsService.SaveSportType(sporttype);
            if (result1 > 0)
                return Json(new { success = true, message = "Sports Type saved successfully!" });
            else
                return Json(new { success = false, message = "Oops. There is some error while saving vehicle. Please try again!" });
        }
        [ActionLog("Sports", "{0} Get Sports Type.")]
        public async Task<IActionResult> GetSportsType(SportsTypeDTO obj)
        {
            var lstsporttype = await _SportsService.GetSportsType(obj.Id);
            return Json(new { success = true, Data = lstsporttype });
        }


    }
}
