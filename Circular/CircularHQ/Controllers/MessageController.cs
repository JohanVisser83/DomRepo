using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.User;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.Message;
using Circular.Services.User;
using CircularHQ.Business;
using CircularHQ.filters;
using CircularHQ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace CircularHQ.Controllers
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
        public HQMessageModel messageModel = new HQMessageModel();
        private readonly ICustomerService _CustomerServives;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CurrentUser currentUser;
        List<string> uploadedpaths = new List<string>();
        public MessageController(IMessageService MessageService, IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration configuration
           , ICommunityService communityService, IHelper helper, IHttpContextAccessor httpContextAccessor, ICustomerRepository customerRepository)
        {

            _MessageService = MessageService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _config = configuration;
            _communityService = communityService ?? throw new ArgumentNullException(nameof(communityService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _global = new Global(_httpContextAccessor, _config, customerRepository);

            //_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //_logger = logger;
        }

        [HttpGet]
        [ActionLog("Message", "{0} opened message.")]
        public async Task<IActionResult> Message()
        {
            try
            {
                CurrentUser currentUser = _global.GetCurrentUser();
                long currentUserId = currentUser.Id;
                long communityId = currentUser.PrimaryCommunityId;
                messageModel.CommunityName = currentUser.PrimaryCommunityName;
                messageModel.CommunityLogo = currentUser.CustomerInfo.PrimaryCommunity.CommunityLogo;
                messageModel.Communities = await _MessageService.GetCommunitiesAsync();

                //messageModel.CommunityFeatures = _communityService.Features(communityId).Result.ToList();

                messageModel.lstMessages = _MessageService.GetAdminChatList(currentUserId).Result.OrderByDescending(m => m.CreatedDate);
                messageModel.lstArchivedMessage = _MessageService.GetAllArchivedMessages(currentUserId).Result.OrderByDescending(m => m.CreatedDate);
                messageModel.lstPolllist = _MessageService.GetAllPoll(communityId).Result.OrderByDescending(n => n.Id);
                messageModel.lstNewsFeeds = _MessageService.GetAllNewFeeds(communityId).Result.OrderByDescending(n => n.Id);
                messageModel.ArchivedArticle = _MessageService.GetAllArchivedArticle(communityId).Result.OrderByDescending(n => n.Id);

                if (messageModel.lstMessages.Count() > 0)
                   messageModel.lstMessageSummary = await _MessageService.GetAdminConversation(_global.currentUser.Id, messageModel.lstMessages.OrderByDescending(m => m.CreatedDate).FirstOrDefault().SenderId);
                if (messageModel.lstArchivedMessage.Count() > 0)
                    messageModel.lstArchivedMessageSummary = await _MessageService.GetAdminConversation(_global.currentUser.Id, messageModel.lstArchivedMessage.OrderByDescending(m => m.CreatedDate).FirstOrDefault().SenderId);
                if (messageModel.lstPolllist.Count() > 0)
                   messageModel.lstPollResult = await _MessageService.GetPoll(messageModel.lstPolllist.FirstOrDefault().Id);
                if (messageModel.ArchivedArticle.Count() > 0)
                    messageModel.lstNewFeedsDetails = _MessageService.GetArchivedArticleDetails(messageModel.ArchivedArticle.FirstOrDefault().Id).Result.FirstOrDefault();
                if (messageModel.lstNewsFeeds.Count() > 0)
                {
                    messageModel.lstActiveNewsFeedDetails = _MessageService.GetActiveArticleDetails(messageModel.lstNewsFeeds.FirstOrDefault().Id).Result.FirstOrDefault();
                    messageModel.lstActiveNewsFeedDetails.DocumentPath = messageModel.lstActiveNewsFeedDetails.DocumentPath.Replace("\\", "/");
                }
                return View(messageModel);
            }
            catch (Exception ex)
            {
                return View();
            }
        }



        [ActionLog("Message", "{0} fetched admin conversations.")]
        public async Task<IActionResult> GetAdminConversations(long Id)
        {
            HQMessageModel model = new HQMessageModel();
            model.lstMessageSummary = await _MessageService.GetAdminConversation(_global.currentUser.Id, Id);
            return PartialView("~/Views/Partials/_HQMessageSummary.cshtml", model);
        }

        [ActionLog("Message", "{0} fetched archive message summary.")]
        public async Task<ActionResult> GetArchivedMessageSummary(long Id)
        {
            HQMessageModel archivedMessageSummary = new HQMessageModel();
            archivedMessageSummary.lstArchivedMessageSummary = await _MessageService.GetAdminConversation(_global.currentUser.Id, Id);
            return PartialView("~/Views/Partials/_HQArchivedMessageSummary.cshtml", archivedMessageSummary);
        }

        [ActionLog("Message", "{0} fetched group list.")]
        public async Task<ActionResult> DropDownCommunityUserList(long CommunityId)
        {
            var key = await _communityService.GetCommunityUser(CommunityId);
            var DropDownUserList = (key?.Select(u => _mapper.Map<CustomerDetails>(u)).ToList());
            return Json(new { Data = DropDownUserList, success = true, Message = "get data" });
        }

        [ActionLog("Message", "{0} fetched user contact and name.")]
        public async Task<ActionResult> GetUserContactList(string Search)
        {
            long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            var userContactList = await _MessageService.GetUserContactListAsync(communityId, Search);
            var result = (userContactList?.Select(u => _mapper.Map<UserContactList>(u)).ToList());
            return Json(result);
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
              //  DateTime dtSchedules = new DateTime(data.Schedule.Year, data.Schedule.Month, data.Schedule.Day, data.ScheduleTime.Value.Hours, data.ScheduleTime.Value.Minutes, 0);

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
                data.Schedule = data.Schedule;
                data.FromId = currentUserId;
                data.MessageExchangeCode = "";
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


        [ActionLog("Message", "{0} fetched active newsfeeds archived article details.")]
        public async Task<ActionResult> GetActiveArticleDetails(long Id)
        {
            HQMessageModel articledetails = new HQMessageModel();
            articledetails.lstActiveNewsFeedDetails = _MessageService.GetActiveArticleDetails(Id).Result.FirstOrDefault();
            articledetails.lstActiveNewsFeedDetails.DocumentPath = articledetails.lstActiveNewsFeedDetails.DocumentPath.Replace("\\", "/");
            return PartialView("~/Views/Partials/_HQActiveArticle.cshtml", articledetails);
        }


        [ActionLog("Message", "{0} fetched inactive newsfeeds archived article details.")]
        public async Task<ActionResult> GetArchivedArticleDetails(long Id)
        {
            try
            {
                HQMessageModel archivedetails = new HQMessageModel();
                archivedetails.lstNewFeedsDetails = _MessageService.GetArchivedArticleDetails(Id).Result.FirstOrDefault();
                return PartialView("~/Views/Partials/_HQArchivedArticle.cshtml", archivedetails);
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        [ActionLog("Message", "{0} fetched group list.")]
        public async Task<ActionResult> DropDownGroupList()
        {
            var key = await _communityService.GetCommunityGroups(_global.currentUser.PrimaryCommunityId);
            var DropDownGroupList = (key?.Select(u => _mapper.Map<Groups>(u)).ToList());
            return Json(DropDownGroupList);
        }


        [ActionLog("Message", "{0} fetched poll options result.")]
        public async Task<ActionResult> GetPollOptionsResult(long pollId, long selectionOptionId)
        {


            HQMessageModel optionResult = new HQMessageModel();

            Poll poll = await _MessageService.GetPoll(pollId);
            optionResult.lstPollResults = poll.Options.Where(o => o.Id == selectionOptionId).FirstOrDefault<PollOptions>().Results;

            if (optionResult != null)
                return Json(new { success = true, message = "", data = optionResult });
            else
                return Json(new { success = false, message = "No Data Found!", data = "" });

        }

        [ActionLog("Message", "{0} fetched poll answer details.")]
        public async Task<ActionResult> GetPollDetails(long PollId)
        {
            try
            {
                HQMessageModel pollList = new HQMessageModel();
                pollList.lstPollResult = await _MessageService.GetPoll(PollId);
                return PartialView("~/Views/Partials/_HQPollResult.cshtml", pollList);
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
    }
}
