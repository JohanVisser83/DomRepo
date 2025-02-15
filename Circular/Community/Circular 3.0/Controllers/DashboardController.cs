using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.Message;
using CircularWeb.Business;
using CircularWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MailKit.Search;
using System.Drawing.Printing;
using Microsoft.Identity.Client;
using Circular.Services.User;
using Circular.Data.Repositories.User;
using CircularWeb.filters;
using Circular.Services.Audit;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace CircularWeb.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IMessageService _MessageService;
        private readonly ICommunityService _communityService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IGlobal _global;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerService _customerService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MessageModel messageModel = new MessageModel();
        public CommunityModel communityModel = new CommunityModel();
        List<string> uploadedpaths = new List<string>();
        private readonly ICustomerRepository _customerRepository;
        private string OIDCUrl;


        public DashboardController(IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, ICommunityService communityService, 
            IHelper helper, IHttpContextAccessor httpContextAccessor, IMessageService messageService,ICustomerRepository customerRepository 
            )
        {
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _config = configuration;
            _communityService = communityService ?? throw new ArgumentNullException(nameof(communityService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _global = new Global(_httpContextAccessor, _config, customerRepository);
            _MessageService = messageService;
            _communityService = _communityService;
            _customerRepository = customerRepository;
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);
        }

        [ActionLog("Dashboard", "{0} opened dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            CurrentUser cuser = _global.GetCurrentUser();
            long Loggedinuser = cuser.Id;
            long primaryId = cuser.PrimaryCommunityId;
            messageModel.CommunityName = cuser.PrimaryCommunityName;
            messageModel.CommunityLogo = cuser.CustomerInfo.PrimaryCommunity.CommunityLogo;
            messageModel.CommunityFeatures =  _communityService.Features(primaryId,Loggedinuser).Result.ToList();
            messageModel.lstCustomerBusinessIndex = await _communityService.GetBusinessIndex(primaryId, null, null, 0, "", 1, 4, true);
            messageModel.lstJobPostingList = await _communityService.GetJobPosting(0, -2, 0, 0, "", primaryId, 1, 100);
            messageModel.lstSponsorInformation =  _customerRepository.GetSponsorInformation(primaryId);
            messageModel.IsOwner = cuser.CustomerInfo.PrimaryCommunity.OwnerCustomerId == Loggedinuser ? true : false;
            messageModel.SubscriptionStatus = cuser.SubscriptionStatus;

            return View(messageModel);
        }



        [ActionLog("Dashboard", "{0} fetched user contact list.")]
        public async Task<ActionResult> GetUserContactList(string Search)
        {
            try
            {
                long communityId = _global.GetCurrentUser().PrimaryCommunityId;
                var userContactLists = await _MessageService.GetUserContactListAsync(communityId, Search);
                var result = (userContactLists?.Select(u => _mapper.Map<UserContactList>(u)).ToList());
                return Json(result);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        [ActionLog("Dashboard", "{0} fetched dropdown list.")]
        public async Task<ActionResult> DropDownGroupList()
        {

            try
            {
                var key = await _communityService.GetCommunityGroups(_global.currentUser.PrimaryCommunityId);
                var DropDownGroupList = (key?.Select(u => _mapper.Map<GroupsDTO>(u)).ToList());

                return Json(DropDownGroupList);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }


        [HttpPost]
        [ActionLog("Dashboard", "{0} saved message.")]
        public async Task<IActionResult> SaveMessage(MessagesDTO data)
        {
            try
            {
                data.FromId = _global.GetCurrentUser().Id;
                data.MessageTypeId = (long)MessageTypeModel.Text;
                if (data.Mediafile != null)
                {
                    if (data.Mediafile.ContentType.Contains("/pdf"))
                        data.MessageTypeId = (long)MessageTypeModel.Pdf;
                    else if (data.Mediafile != null && data.Mediafile.ContentType.Contains("image/"))
                        data.MessageTypeId = (long)MessageTypeModel.Image;
                    else if (data.Mediafile != null && data.Mediafile.ContentType.Contains("media")) // check for video
                        data.MessageTypeId = (long)MessageTypeModel.Video;
                    data.MessageMedia = _helper.SaveFile(data.Mediafile, _global.UploadFolderPath, this.Request);
                }
                Messages messages = _mapper.Map<Messages>(data);
                messages.IsNewMessage = true;
                messages.IsPaid = false;
                messages.MessageExchangeCode = "";
                messages.IsCommunityPortalMessage = 1;
                messages.CommunityName = _global.GetCurrentUser().PrimaryCommunityName;
                var result = await _MessageService.Save(messages);
                if (result is not null)
                    return Json(new { success = true, message = "" });
                else
                    return Json(new { success = false, message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "" }); ;
            }
        }

        [ActionLog("Dashboard", "{0} saved schedule message.")]
        public async Task<IActionResult> SaveScheduleMessage(MessageSchedule data)
        {
            try
            {
                long currentUserId = _global.GetCurrentUser().Id;
                data.MessageTypeId = (long)MessageTypeModel.Text;

                if (data.Mediafile != null)
                {
                    if (data.Mediafile.ContentType.Contains("/pdf"))
                        data.MessageTypeId = (long)MessageTypeModel.Pdf;
                    else if (data.Mediafile != null && data.Mediafile.ContentType.Contains("image/"))
                        data.MessageTypeId = (long)MessageTypeModel.Image;
                    else if (data.Mediafile != null && data.Mediafile.ContentType.Contains("media")) // check for video
                        data.MessageTypeId = (long)MessageTypeModel.Video;
                    data.MessageMedia = _helper.SaveFile(data.Mediafile, _global.UploadFolderPath, this.Request);
                }
                data.FromId = currentUserId;
                data.CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                MessageSchedule Schedulemessages = _mapper.Map<MessageSchedule>(data);
                var result = await _MessageService.SaveScheduleMessage(Schedulemessages);
                if (result > 0)
                    return Json(new { success = true, message = "" });
                else
                    return Json(new { success = false, message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "" }); ;
            }
        }

        [ActionLog("Dashboard", "{0} sended broadcast messages")]

        public async Task<ActionResult> SendBroadcastMessage(BroadcastDTO broadcastDTO)
        {
            try
            {
                var communityName = _global.currentUser.PrimaryCommunityName;
                var UserId = _global.currentUser.Id;
                broadcastDTO.MessageTypeId = (long)MessageTypeModel.Text;

                if (broadcastDTO.Mediafile != null)
                {
                    if (broadcastDTO.Mediafile.ContentType.Contains("/pdf"))
                        broadcastDTO.MessageTypeId = (long)MessageTypeModel.Pdf;
                    else if (broadcastDTO.Mediafile != null && broadcastDTO.Mediafile.ContentType.Contains("image/"))
                        broadcastDTO.MessageTypeId = (long)MessageTypeModel.Image;
                    else if (broadcastDTO.Mediafile != null && broadcastDTO.Mediafile.ContentType.Contains("media")) // check for video
                        broadcastDTO.MessageTypeId = (long)MessageTypeModel.Video;
                    broadcastDTO.MessageMedia = _helper.SaveFile(broadcastDTO.Mediafile, _global.UploadFolderPath, this.Request);

                }

                broadcastDTO.ScheduleDateTime = broadcastDTO.ScheduleDateTime.ToString("yyyy-MM-dd") == "0001-01-01" ? DateTime.Now : (DateTime)broadcastDTO.ScheduleDateTime;
              //  broadcastDTO.ScheduleTime = broadcastDTO.ScheduleTime.HasValue ? (TimeSpan)broadcastDTO.ScheduleTime.Value : DateTime.Now.TimeOfDay;               
                broadcastDTO.ScheduleTime = broadcastDTO.ScheduleTime.HasValue ? (DateTime)broadcastDTO.ScheduleTime.Value : DateTime.Now.ToLocalTime();               
                //DateTime dtSchedules = new DateTime(broadcastDTO.ScheduleDateTime.Year, broadcastDTO.ScheduleDateTime.Month, broadcastDTO.ScheduleDateTime.Day, broadcastDTO.ScheduleTime.Value.Hours, broadcastDTO.ScheduleTime.Value.Minutes, 0);
                DateTime dtSchedules = new DateTime(broadcastDTO.ScheduleDateTime.Year, broadcastDTO.ScheduleDateTime.Month, broadcastDTO.ScheduleDateTime.Day, broadcastDTO.ScheduleTime.Value.Hour, broadcastDTO.ScheduleTime.Value.Minute, 0);
                

                broadcastDTO.ScheduleDateTime = dtSchedules;

                //if (broadcastDTO.ScheduleDateTime == null && broadcastDTO.ScheduleTime == null)
                //{
                //    broadcastDTO.ScheduleDateTime = broadcastDTO.ScheduleDateTime ?? DateTime.Now;

                //}
                //else
                //{
                //    DateTime dtSchedules = new DateTime(broadcastDTO.ScheduleDateTime.Value.Year, broadcastDTO.ScheduleDateTime.Value.Month, broadcastDTO.ScheduleDateTime.Value.Day, broadcastDTO.ScheduleTime.Value.Hours, broadcastDTO.ScheduleTime.Value.Minutes, 0);
                //    broadcastDTO.ScheduleDateTime = dtSchedules;
                //}

                if (broadcastDTO.Type == "all")
                {
                    broadcastDTO.IsGroup = 0;
                    broadcastDTO.ReferenceId = 0;
                }
                else
                {
                    broadcastDTO.IsGroup = 1;
                }

                broadcastDTO.CommunityName = communityName;
                broadcastDTO.CustomerId = UserId;
                broadcastDTO.CreatedBy = UserId;
                broadcastDTO.ModifiedBy = UserId;
                broadcastDTO.CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                Broadcast broadcastMessage = _mapper.Map<Broadcast>(broadcastDTO);
                var result = await _MessageService.SaveBroadcastMessage(broadcastMessage);

                if (result > 0)
                    return Json(new { success = true, message = "" });
                else
                    return Json(new { success = false, message = "" });
            }
            catch (Exception ex)
            {
                return null;
            }


        }




        [ActionLog("Dashboard", "{0} saved message.")]

        public async Task<IActionResult> SendNewFeeds(NewsFeedsDTO newsFeedsDTO)
        {
            try
            {
                List<string> imageList = new List<string>();
                long loggedInUserId = _global.GetCurrentUser().Id;
                var communityName = _global.currentUser.PrimaryCommunityName;
                string uploadpdffile = string.Empty, CoverImage = string.Empty, imgListMultiple = string.Empty;
                string updatedcommunityName = communityName.Replace("-", " ").Replace("@", "").Replace("(", "").Replace(")", "").Replace(",", "").Replace("~", "").Replace("#", "").Replace("$", "").Replace("*", "").Replace("^", "").Replace("!", "").Replace("'", "");
                string UploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString() + "/Articles/" + updatedcommunityName.Trim();


                if (newsFeedsDTO.Mediafile != null && newsFeedsDTO.ArticleMedia.Count == 1)
                {
                    if (newsFeedsDTO.ArticleMedia[0].MediaType.Contains("image"))
                    {
                        string imagefiles = newsFeedsDTO.ArticleMedia[0].filename;
                        string ImgBase64s = newsFeedsDTO.ArticleMedia[0].Media;
                        string imagepaths = "";
                        //newsFeedsDTO.ArticleMedia[0].Media = _helper.SaveFile(newsFeedsDTO.Mediafile, UploadFolder, this.Request);
                        //newsFeedsDTO.ArticleMedia[0].Media = _helper.ConvertBase64toImage(imagefiles, ImgBase64s, UploadFolder, this.Request);
                        //newsFeedsDTO.ArticleMedia[0].MediaType = "Image";
                        //newsFeedsDTO.ArticleMedia[0].CreatedBy = loggedInUserId;
                        //newsFeedsDTO.ArticleMedia[0].ModifiedBy = loggedInUserId;
                        //newsFeedsDTO.ArticleMedia[0].CreatedDate = DateTime.Now;
                        //newsFeedsDTO.ArticleMedia[0].ModifiedDate = DateTime.Now;
                        //newsFeedsDTO.ArticleMedia[0].IsActive = true;
                    }
                    else
                    {
                        newsFeedsDTO.ArticleMedia[0].MediaType = "Video";
                        string imagefile = newsFeedsDTO.ArticleMedia[0].filename;
                        string ImgBase64 = newsFeedsDTO.ArticleMedia[0].Media;
                        string imagepath = "";
                        if (newsFeedsDTO.ArticleMedia[0].MediaType == "Image")
                            imagepath = _helper.ConvertBase64toImage(imagefile, ImgBase64, UploadFolder, this.Request);
                        else
                            imagepath = _helper.ConvertBase64toMp4(imagefile, ImgBase64, UploadFolder, this.Request);

                        newsFeedsDTO.ArticleMedia[0].Media = Convert.ToString(imagepath);
                        newsFeedsDTO.ArticleMedia[0].CreatedBy = loggedInUserId;
                        newsFeedsDTO.ArticleMedia[0].ModifiedBy = loggedInUserId;
                        newsFeedsDTO.ArticleMedia[0].CreatedDate = DateTime.Now;
                        newsFeedsDTO.ArticleMedia[0].ModifiedDate = DateTime.Now;
                        newsFeedsDTO.ArticleMedia[0].IsActive = true;
                    }

                }
                else
                {
                    for (int i = 0; i < newsFeedsDTO.ArticleMedia.Count; i++)
                    {
                        string imagefile = newsFeedsDTO.ArticleMedia[i].filename;
                        string ImgBase64 = newsFeedsDTO.ArticleMedia[i].Media;
                        if (newsFeedsDTO.ArticleMedia[i].MediaType.Contains("image"))
                            newsFeedsDTO.ArticleMedia[i].MediaType = "Image";
                        else
                            newsFeedsDTO.ArticleMedia[i].MediaType = "Video";
                        string imagepath = "";
                        if (newsFeedsDTO.ArticleMedia[i].MediaType == "Image")
                            imagepath = _helper.ConvertBase64toImage(imagefile, ImgBase64, UploadFolder, this.Request);
                        else
                            imagepath = _helper.ConvertBase64toMp4(imagefile, ImgBase64, UploadFolder, this.Request);

                        newsFeedsDTO.ArticleMedia[i].Media = Convert.ToString(imagepath);

                        newsFeedsDTO.ArticleMedia[i].CreatedBy = loggedInUserId;
                        newsFeedsDTO.ArticleMedia[i].ModifiedBy = loggedInUserId;
                        newsFeedsDTO.ArticleMedia[i].CreatedDate = DateTime.Now;
                        newsFeedsDTO.ArticleMedia[i].ModifiedDate = DateTime.Now;
                        newsFeedsDTO.ArticleMedia[i].IsActive = true;



                    }


                }


                if (newsFeedsDTO.Mediafile != null)
                {
                    var file = Request.Form.Files[0];
                    SavemultipleFile(file, UploadFolder, this.Request);
                    var data = uploadedpaths;
                    if (data.Count() > 0)
                    {
                        CoverImage = data[0];
                        newsFeedsDTO.MessageMediaThumbnail = Convert.ToString(CoverImage);
                        newsFeedsDTO.MessageMedia = Convert.ToString(CoverImage);
                        //newsFeedsDTO.ArticleMedia[0].Media = Convert.ToString(CoverImage);
                        //newsFeedsDTO.ArticleMedia[0].MediaType = "Image";
                        //newsFeedsDTO.ArticleMedia[0].CreatedBy = loggedInUserId;
                        //newsFeedsDTO.ArticleMedia[0].ModifiedBy = loggedInUserId;
                        //newsFeedsDTO.ArticleMedia[0].CreatedDate = DateTime.Now;
                        //newsFeedsDTO.ArticleMedia[0].ModifiedDate = DateTime.Now;
                        //newsFeedsDTO.ArticleMedia[0].IsActive = true;

                        if (newsFeedsDTO.ArticleMedia.Count == 1)
                        {
                            newsFeedsDTO.ArticleMedia[0].Media = Convert.ToString(CoverImage);
                            newsFeedsDTO.ArticleMedia[0].MediaType = "Image";
                            newsFeedsDTO.ArticleMedia[0].CreatedBy = loggedInUserId;
                            newsFeedsDTO.ArticleMedia[0].ModifiedBy = loggedInUserId;
                            newsFeedsDTO.ArticleMedia[0].CreatedDate = DateTime.Now;
                            newsFeedsDTO.ArticleMedia[0].ModifiedDate = DateTime.Now;
                            newsFeedsDTO.ArticleMedia[0].IsActive = true;
                        }
                        else
                        {

                        }

                    }
                    if (data.Count() > 1)
                    {
                        uploadpdffile = data[1];
                    }





                }

                newsFeedsDTO.CustomerId = loggedInUserId;
                //newsFeedsDTO.MessageMediaThumbnail = Convert.ToString(CoverImage);
                //newsFeedsDTO.MessageMedia = Convert.ToString(CoverImage);
                newsFeedsDTO.DocumentPath = Convert.ToString(uploadpdffile);
                newsFeedsDTO.CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                newsFeedsDTO.CommunityName = communityName;
                newsFeedsDTO.CreatedBy = loggedInUserId;
                newsFeedsDTO.ModifiedBy = loggedInUserId;
                newsFeedsDTO.ReferenceTypeId = 0;
                newsFeedsDTO.IsArchived = false;

                if (newsFeedsDTO.Type == "all")
                {
                    newsFeedsDTO.IsGroup = 0;
                    newsFeedsDTO.ReferenceId = 0;
                }
                else
                {
                    newsFeedsDTO.IsGroup = 1;
                }
                NewsFeeds article = _mapper.Map<NewsFeeds>(newsFeedsDTO);
                var result = await _MessageService.SendNewFeeds(article);
                if (result > 0)
                    return Json(new { success = true, message = "" });
                else
                    return Json(new { success = false, message = "" });
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        public void SavemultipleFile(IFormFile file, string uploadFolder, HttpRequest request)
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

                var uniqueFileName = GetUniqueFileName(file.FileName);
                //string fileName = Path.GetFileName(uniqueFileName);

                path = Path.Combine(filesPath, uniqueFileName);
                file.CopyToAsync(new FileStream(path, FileMode.Create));
                returnfilepath = Path.Combine(url, uniqueFileName);
                uploadedpaths.Add(returnfilepath);
            }

        }
        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

        [ActionLog("Dashboard", "{0} fetched business index.")]
        public async Task<IActionResult> GetDashboardBusinessIndex(long BusinessIndexId)
        {
            long primaryId = _global.GetCurrentUser().PrimaryCommunityId;
            var result = await _communityService.GetBusinessIndex(primaryId, BusinessIndexId, null, 0, "", 0, 100,true);
            if (result?.Count() > 0)
            {
                return Json(new { success = true, message = "Data Save Successfully!", data = result });
            }
            else
            {
                return Json(new { success = false, message = "Data  not Save Successfully!" });
            }
        }
        [ActionLog("Dashboard", "{0} fetched job posting.")]
        public async Task<IActionResult> GetDashboardJobPosting(long IsApproved)
        {
            long primaryId = _global.GetCurrentUser().PrimaryCommunityId;
            var result = await _communityService.GetJobPosting(0, IsApproved, 0, 0, "", primaryId, 0, 100);
            if(result?.Count() > 0) 
            {
                return Json(new { success = true, message = "Data Save Successfully!", data = result });
            }
            else
            {
                return Json(new { success = false, message = "Data not Save Successfully!"});
            }

        }
        [ActionLog("Dashboard", "{0} business approved.")]
        public async Task<IActionResult> DashboardIsBusinessApprove(long Id)
        {
            long comunityId = _global.currentUser.PrimaryCommunityId;
            var result = await _communityService.ChangeIsBusinessApproved(Id, true, comunityId);
            if (result > 0)
                return Json(new { success = true, message = "" });
            else
                return Json(new { success = false, message = "" });
        }
        [ActionLog("Dashboard", "{0} business declined.")]
        public async Task<IActionResult> DeclineBusiness(long Id, bool IsBusinessApproved)
        {
            long comunityId = _global.currentUser.PrimaryCommunityId;
            var result = await _communityService.ChangeIsBusinessApproved(Id, false, comunityId);
            if (result > 0)
                return Json(new { success = true, message = "" });
            else
                return Json(new { success = false, message = "" });
        }
        [ActionLog("Dashboard", "{0} job approved.")]
        public async Task<IActionResult> DashboardIsJobApprove(long Id)
        {
                long LoggedInUserId = _global.currentUser.PrimaryCommunityId;
                var result = await _communityService.ChangeIsJobApproved(Id, true, LoggedInUserId);
                if ( result > 0)
                    return Json(new { success = true, message = "" });
                else
                    return Json(new { success = false, message = "" });
        }
        [ActionLog("Dashboard", "{0} updated job approve.")]
        public async Task<IActionResult> UpdateJobApproved(long Id, bool IsApproved)
        {
            try
            {
                long LoggedInUserId = _global.currentUser.PrimaryCommunityId;

                var result = await _communityService.ChangeIsJobApproved(Id, IsApproved, LoggedInUserId);
                if (result > 0)
                {
                    return Json(new { success = true, message = "" });
                }
                else
                {
                    return Json(new { success = false, message = "" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "" });
            }
        }


    }
}
