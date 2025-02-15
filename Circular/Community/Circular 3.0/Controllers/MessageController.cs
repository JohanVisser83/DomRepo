using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.Message;
using CircularWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CircularWeb.Business;
using Circular.Services.User;
using RepoDb.Extensions;
using MailKit.BounceMail;
using FirebaseAdmin.Auth;
using Circular.Data.Repositories.User;
using CircularWeb.filters;
using NLog;



namespace CircularWeb.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IMessageService _MessageService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IGlobal _global;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICommunityService _communityService;
        public MessageModel messageModel = new MessageModel();
        private readonly ICustomerService _CustomerServives;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CurrentUser currentUser;
        List<string> uploadedpaths = new List<string>();
        private string OIDCUrl;

        public MessageController(IMessageService MessageService, IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration configuration
            , ICommunityService communityService,IHelper helper, IHttpContextAccessor httpContextAccessor, ICustomerRepository customerRepository)
        {
           
            _MessageService = MessageService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _config = configuration;
            _communityService = communityService ?? throw new ArgumentNullException(nameof(communityService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _global = new Global(_httpContextAccessor, _config,customerRepository);
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);
            //_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //_logger = logger;
        }

        [HttpGet]
        [ActionLog("Message", "{0} opened message.")]

        #region "Messages"
        public async Task<IActionResult> Message()
        {
            try
            {
                CurrentUser currentUser = _global.GetCurrentUser();
                long currentUserId = currentUser.Id;
                long communityId = currentUser.PrimaryCommunityId;
                messageModel.CommunityName = currentUser.PrimaryCommunityName;
                messageModel.CommunityLogo = currentUser.CustomerInfo.PrimaryCommunity.CommunityLogo;
                messageModel.CommunityFeatures = _communityService.Features(communityId,currentUserId).Result.ToList();

                messageModel.lstMessages =   _MessageService.GetAdminChatList(currentUserId).Result.OrderByDescending(m => m.CreatedDate);
                messageModel.lstbroadcastMessage =  _MessageService.GetAllBroadcastMessage(communityId).Result.OrderByDescending(b=>b.CreatedDate);
                messageModel.lstArchivedMessage = _MessageService.GetAllArchivedMessages(currentUserId).Result.OrderByDescending(m=>m.CreatedDate);
                messageModel.lstPolllist =  _MessageService.GetAllPoll(communityId).Result.OrderByDescending(n=> n.Id);
                messageModel.lstNewsFeeds =  _MessageService.GetAllNewFeeds(communityId).Result.OrderByDescending(n=>n.Id);
                messageModel.ArchivedArticle =  _MessageService.GetAllArchivedArticle(communityId).Result.OrderByDescending(n => n.Id);
                messageModel.IsOwner = currentUser.CustomerInfo.PrimaryCommunity.OwnerCustomerId == currentUserId ? true : false;

                if (messageModel.lstMessages.Count() > 0)
                    messageModel.lstMessageSummary = await _MessageService.GetAdminConversation(_global.currentUser.Id, messageModel.lstMessages.OrderByDescending(m=>m.CreatedDate).FirstOrDefault().SenderId);
                if (messageModel.lstbroadcastMessage.Count() > 0)
                    messageModel.broadcastSummaryDetails = await _MessageService.GetBroadcastSummary(messageModel.lstbroadcastMessage.FirstOrDefault().Id);
               if (messageModel.lstArchivedMessage.Count() > 0)
                    messageModel.lstArchivedMessageSummary = await _MessageService.GetAdminConversation(_global.currentUser.Id, messageModel.lstArchivedMessage.OrderByDescending(m=>m.CreatedDate).FirstOrDefault().SenderId);
                if (messageModel.lstPolllist.Count() > 0)
                    messageModel.lstPollResult = await _MessageService.GetPoll(messageModel.lstPolllist.FirstOrDefault().Id);
                if (messageModel.ArchivedArticle.Count() > 0)
                    messageModel.lstNewFeedsDetails =  _MessageService.GetArchivedArticleDetails(messageModel.ArchivedArticle.FirstOrDefault().Id).Result.FirstOrDefault();
                if (messageModel.lstNewsFeeds.Count() > 0)
                {
                    messageModel.lstActiveNewsFeedDetails = _MessageService.GetActiveArticleDetails(messageModel.lstNewsFeeds.FirstOrDefault().Id).Result.FirstOrDefault();
                    messageModel.lstActiveNewsFeedDetails.DocumentPath = messageModel.lstActiveNewsFeedDetails.DocumentPath.Replace("\\", "/");
                }
                messageModel.SubscriptionStatus = currentUser.SubscriptionStatus;
                if (!messageModel.IsFeatureAvailable("M-003"))
                throw new ArgumentNullException("Unauthroized : You dont have permission to access this functionality.");
                
                else
                    return View(messageModel);

                
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        [ActionLog("Message", "{0} saved message.")]
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
                    else if (data.Mediafile != null && data.Mediafile.ContentType.Contains("video/")) // check for video
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

        [ActionLog("Message", "{0} saved schedule message.")]
        public async Task<IActionResult> SaveScheduleMessage(MessageSchedule data)
        {
            try
            {
              // DateTime dtSchedules = new DateTime(data.Schedule.Year,data.Schedule.Month,data.Schedule.Day,data.ScheduleTime.Value.Hours,data.ScheduleTime.Value.Minutes,0);
                
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
                // data.Schedule = dtSchedules;
                data.Schedule = data.Schedule;

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

        [ActionLog("Message", "{0} fetched user contact and name.")]
        public async Task<ActionResult> GetUserContactList(string Search)
   {
            long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            var userContactList = await _MessageService.GetUserContactListAsync(communityId, Search);
            var result = (userContactList?.Select(u => _mapper.Map<UserContactList>(u)).ToList());
            return Json(result);
        }

        [ActionLog("Message", "{0} fetched admin conversations.")]
        public async Task<IActionResult> GetAdminConversations(long Id)
        {
            MessageModel model = new MessageModel();
            model.lstMessageSummary = await _MessageService.GetAdminConversation(_global.currentUser.Id, Id);
            return PartialView("~/Views/Partials/_MessageSummary.cshtml", model);
        }

        #endregion



        [ActionLog("Message", "{0} fetched group list.")]
        public async Task<ActionResult> DropDownGroupList()
        {
                    var key = await _communityService.GetCommunityGroups(_global.currentUser.PrimaryCommunityId);
                    var DropDownGroupList = (key?.Select(u => _mapper.Map<Groups>(u)).ToList());
                    return Json(DropDownGroupList);
        }


        [HttpPost]
        [ActionLog("Message", "{0} sended broadcast message.")]
		public async Task<ActionResult> SendBroadcastMessage(BroadcastDTO broadcastDTO)
		{
			var logger = LogManager.GetLogger("database");
			logger.Info("loggerTest broadcast");

			try
			{
				var communityName = _global.currentUser.PrimaryCommunityName;
				var UserId = _global.currentUser.Id;
				broadcastDTO.MessageTypeId = (long)MessageTypeModel.Text;

				// Handle media file
				if (broadcastDTO.Mediafile != null)
				{
					if (broadcastDTO.Mediafile.ContentType.Contains("/pdf"))
						broadcastDTO.MessageTypeId = (long)MessageTypeModel.Pdf;
					else if (broadcastDTO.Mediafile.ContentType.Contains("image/"))
						broadcastDTO.MessageTypeId = (long)MessageTypeModel.Image;
					else if (broadcastDTO.Mediafile.ContentType.Contains("media"))
						broadcastDTO.MessageTypeId = (long)MessageTypeModel.Video;

					broadcastDTO.MessageMedia = _helper.SaveFile(broadcastDTO.Mediafile, _global.UploadFolderPath, this.Request);
				}

				// ScheduleDateTime and ScheduleTime handling
				try
				{
					DateTime currentDateTime = DateTime.Now;
					DateTime scheduleDateTime;

					// Default ScheduleDateTime to now if it's not set or is the default value
					if (broadcastDTO.ScheduleDateTime == default(DateTime))
					{
						scheduleDateTime = currentDateTime;
					}
					else
					{
						// If ScheduleDateTime is set, combine it with ScheduleTime
						scheduleDateTime = broadcastDTO.ScheduleDateTime;

						// If ScheduleTime is provided, use it; otherwise, default to current time
						if (broadcastDTO.ScheduleTime.HasValue)
						{
							scheduleDateTime = new DateTime(
								scheduleDateTime.Year,
								scheduleDateTime.Month,
								scheduleDateTime.Day,
								broadcastDTO.ScheduleTime.Value.Hour,
								broadcastDTO.ScheduleTime.Value.Minute,
								0
							);
						}
						else
						{
							// Use current time if ScheduleTime is not provided
							scheduleDateTime = new DateTime(
								scheduleDateTime.Year,
								scheduleDateTime.Month,
								scheduleDateTime.Day,
								currentDateTime.Hour,
								currentDateTime.Minute,
								0
							);
						}
					}

					broadcastDTO.ScheduleDateTime = scheduleDateTime;
				}
				catch (Exception ex)
				{
					logger.Info("Error processing ScheduleDateTime and ScheduleTime: " + ex.Message);
					return Json(new { success = false, message = "Error processing scheduling information." });
				}

				// Set other broadcastDTO properties
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

				// Map to Broadcast entity and save
				Broadcast broadcastMessage = _mapper.Map<Broadcast>(broadcastDTO);
				var result = await _MessageService.SaveBroadcastMessage(broadcastMessage);
				logger.Info("Inside SaveBroadcastMessage");

				if (result > 0)
					return Json(new { success = true, message = "" });
				else
					return Json(new { success = false, message = "Failed to save broadcast message." });
			}
			catch (Exception ex)
			{
				logger.Info("Error sending broadcast: " + ex.Message);
				return Json(new { success = false, message = "An error occurred while sending the broadcast message." });
			}
		}

		[ActionLog("Message", "{0} fetched broadcast list.")]
        public async Task<ActionResult> GetBroadcastDetail(long Id)
        {
            MessageModel broadcastdetails = new MessageModel();
            broadcastdetails.broadcastSummaryDetails = await _MessageService.GetBroadcastSummary(Id);
            return PartialView("~/Views/Partials/_BroadCastMessageSummary.cshtml", broadcastdetails);
        }

        [ActionLog("Message", "{0} fetched archive message summary.")]
        public async Task<ActionResult> GetArchivedMessageSummary(long Id)
        {
            MessageModel archivedMessageSummary = new MessageModel();
            archivedMessageSummary.lstArchivedMessageSummary = await _MessageService.GetAdminConversation(_global.currentUser.Id, Id);
            return PartialView("~/Views/Partials/_ArchivedMessageSummary.cshtml", archivedMessageSummary);
        }
        [ActionLog("Message", "{0} archived message.")]
        public async Task<IActionResult> ArchivedMessage(long UserId)
        {
            try
            {
                var result = await _MessageService.ArchivedMessage(_global.currentUser.Id, UserId);

                if (result != null)
                {
                    return Json(new { success = true, message = "Message Archived Successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "OOPS! Something Went Wrong!" });
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        [ActionLog("Message", "{0} sended poll.")]
        public async Task<ActionResult> SendPoll(Poll poll)
        {
            var UserId = _global.currentUser.Id;
            var communityId = _global.currentUser.PrimaryCommunityId;
            var communityName = _global.currentUser.PrimaryCommunityName;

            if (poll.Type == "all")
            {

                poll.CommunityId = communityId;
                poll.IsGroup = 0;
                poll.CommunityName = communityName;
                //poll.ReferenceId = UserId; 

                Poll pollItems = _mapper.Map<Poll>(poll);
                var result = await _MessageService.SendNewPoll(UserId, pollItems);
                return Json(result);
            }
            else
            {

                poll.CommunityId = communityId;
                poll.IsGroup = 1;
                poll.CommunityName = communityName;
                //poll.ReferenceId = UserId;
                Poll pollItems = _mapper.Map<Poll>(poll);
                var result = await _MessageService.SendNewPoll(UserId, pollItems);
                return Json(result);
            }
        }

        [ActionLog("Message", "{0} fetched poll answer details.")]
        public async Task<ActionResult> GetPollDetails(long PollId)
        {
            try
            {
                MessageModel pollList = new MessageModel();
                pollList.lstPollResult = await _MessageService.GetPoll(PollId);
                return PartialView("~/Views/Partials/_PollResult.cshtml", pollList);
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        [ActionLog("Message", "{0} deleted poll.")]
        public async Task<ActionResult> DeletePoll(long? id)
        {

            long pollItem = await _MessageService.DeletePoll(id);

            if (pollItem > 0)
                return Json(new { success = true, message = "Poll Deleted Successfully!" });
            else
                return Json(new { success = false, message = "Poll Deleted not Successfully!" });
        }

        [ActionLog("Message", "{0} fetched poll options result.")]
        public async Task<ActionResult> GetPollOptionsResult(long pollId, long selectionOptionId)
        {
            MessageModel optionResult = new MessageModel();

            Poll poll= await _MessageService.GetPoll(pollId);
            optionResult.lstPollResults = poll.Options.Where(o => o.Id == selectionOptionId).FirstOrDefault<PollOptions>().Results;

            if (optionResult != null)
                return Json(new { success = true, message = "", data = optionResult });
            else
                return Json(new { success = false, message = "No Data Found!", data = "" });

        }


        [ActionLog("Message", "{0} saved new feeds.")]
        public async Task<IActionResult> SendNewFeeds(NewsFeedsDTO newsFeedsDTO)
        {
            try
            {
                List<string> imageList = new List<string>();
                long loggedInUserId = _global.GetCurrentUser().Id;
                var communityName = _global.currentUser.PrimaryCommunityName;
                string uploadpdffile = string.Empty, CoverImage = string.Empty, imgListMultiple = string.Empty;
                string updatedcommunityName = communityName.Replace("-", "").Replace("@","").Replace("(", "").Replace(")", "").Replace(",", "").Replace("~","").Replace("#","").Replace("$","").Replace("*","").Replace("^","").Replace("!","").Replace("'","");
                string UploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString() + "/Articles/" + updatedcommunityName.Trim();
                

                if (newsFeedsDTO.Mediafile != null && newsFeedsDTO.ArticleMedia.Count == 1)
                {
                    if (newsFeedsDTO.ArticleMedia[0].MediaType.Contains("image"))
                    {
                        string imagefiles = newsFeedsDTO.ArticleMedia[0].filename;
                        string ImgBase64s = newsFeedsDTO.ArticleMedia[0].Media;
                        string imagepaths = "";
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
                        if (newsFeedsDTO.ArticleMedia.Count == 1 )
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


        public string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName).Replace(" ", "_").Replace("(", "").Replace(")", "")
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + "_" + DateTime.Now.ToString("yyyyMMddmmss")
                      + Path.GetExtension(fileName);
        }

        [ActionLog("Message", "{0} fetched archived article.")]
        public async Task<IActionResult> ArchivedArticle(long Id)
        {
            try
            {
                var result = await _MessageService.ArchivedArticle(Id, _global.currentUser.Id);

                if (result != null)
                {
                    return Json(new { success = true, message = "Article Archived Successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "OOPS! Something Went Wrong!" });
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IActionResult> FeatureArticle(long Id)
        {
            try
            {
                var result = await _MessageService.FeatureArticle(Id, _global.currentUser.Id);

                if (result != null)
                {
                    return Json(new { success = true, message = "Article Feature Successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "OOPS! Something Went Wrong!" });
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IActionResult> UnFeatureArticle(long Id)
        {
            try
            {
                var result = await _MessageService.UnFeatureArticle(Id, _global.currentUser.Id);

                if (result != null)
                {
                    return Json(new { success = true, message = "Article UnFeature Successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "OOPS! Something Went Wrong!" });
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [ActionLog("Message", "{0} unArchived message.")]
        public async Task<IActionResult> UnArchivedMessage(long ArchivedUserid)
        {
            var UnarchivedItem = await _MessageService.UnArchivedMessage(ArchivedUserid, _global.currentUser.Id);
            if (UnarchivedItem != null)
            {
                return Json(new { success = true, message = "Message UnArchived Successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "OOPS! Something Went Wrong!" });
            }
        }

        [ActionLog("Message", "{0} unArchived article.")]
        public async Task<IActionResult> UnArchivedArticle(long ArchivedUserid)
        {
            var UnarchivedarticleItem = await _MessageService.UnArchivedArticle(ArchivedUserid, _global.currentUser.Id);
            if (UnarchivedarticleItem != null)
            {
                return Json(new { success = true, message = "Article UnArchived Successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "OOPS! Something Went Wrong!" });
            }
        }

        [ActionLog("Message", "{0} fetched inactive newsfeeds archived article details.")]
        public async Task<ActionResult> GetArchivedArticleDetails(long Id)
        {
            try
            {
                MessageModel archivedetails = new MessageModel();
                archivedetails.lstNewFeedsDetails =  _MessageService.GetArchivedArticleDetails(Id).Result.FirstOrDefault();
                return PartialView("~/Views/Partials/_Archivearticle.cshtml", archivedetails);
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        [ActionLog("Message", "{0} fetched active newsfeeds archived article details.")]
        public async Task<ActionResult> GetActiveArticleDetails(long Id)
        {
            MessageModel articledetails = new MessageModel();
            articledetails.lstActiveNewsFeedDetails =  _MessageService.GetActiveArticleDetails(Id).Result.FirstOrDefault();
            articledetails.lstActiveNewsFeedDetails.DocumentPath = articledetails.lstActiveNewsFeedDetails.DocumentPath.Replace("\\","/");
            return PartialView("~/Views/Partials/_ActiveArticle.cshtml", articledetails);
        }



    }


}
