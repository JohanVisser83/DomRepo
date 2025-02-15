using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;
using Circular.Filters;
using Circular.Framework.Logger;
using Circular.Services.Message;
using Circular.Services.Notifications;
using Circular.Services.User;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Circular.Services.Community;
using NLog;

namespace Circular.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMessageService _messageService;
        private readonly ICommunityService _communityService;
        private readonly ILoggerManager _logger;
        private readonly INotificationService _notificationService;
        private readonly ICommon _common;
        List<string> uploadedpaths = new List<string>();

        public MessageController(IMapper mapper, IMessageService messageService, ICommunityService communityService, ILoggerManager logger,ICustomerService customerService,
            INotificationService notificationService, ICommon common)
        {
            _common = common;
            _mapper = mapper;
            _messageService = messageService;
            _communityService = communityService ?? throw new ArgumentNullException(nameof(communityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notificationService = notificationService;
        }


        [HttpGet]
        [AuthorizeOIDC]
        [Route("Chats")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Message", "{UserName} Requested Chats")]
		public async Task<ActionResult<APIResponse>> GetChats(long LoggedinUserId)
        {
            IEnumerable<GetConversation> messageSummary = await _messageService.GetChats(LoggedinUserId);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = messageSummary;
            return Ok(apiResponse);
        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Chats/Conversation")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Message", "{UserName} Requested Conversation")]
		public async Task<ActionResult<APIResponse>> GetConversation([FromBody] GetMessagesRequest getMessagesRequest)
        {

            MessagesListResponse messagesListResponse = await _messageService.GetConversation(getMessagesRequest.Customerid, getMessagesRequest.SelectedCustomerId);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = messagesListResponse;
            return Ok(apiResponse);
        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Send")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Message", "{UserName} Send Message")]
		public async Task<IActionResult> Send([FromBody] SaveMessagesRequest saveMessagesRequest)
        {
            try
            {
                Messages message = _mapper.Map<Messages>(saveMessagesRequest);
                message.IsNewMessage = true;
                message.Message = message.Message?.Replace("'", "''");
                message.CommunityId = _common.CurrentUser().PrimaryCommunity.CommunityId ?? 0;
                Messages _messages = await _messageService.Save(message);

                MessagesDTO messageDTO = _mapper.Map<MessagesDTO>(_messages);
                APIResponse apiResponse = new APIResponse();
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = messageDTO;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogInfo(ex.Message);
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Read")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Message", "{UserName} Read Message")]
		public async Task<IActionResult> Read([FromBody] ReadMessageRequest readMessageRequest)
        {
            try
            {
                MessageSummary _readMessage = new MessageSummary() 
                { LastReadMessageId = readMessageRequest.LastReadMessageId, 
                    SenderId=readMessageRequest.SelectedUserId,ReceiverId=readMessageRequest.ReceiverId };
                var result = await _messageService.ReadMessage(_readMessage);
                APIResponse clsResponse = new APIResponse();
                clsResponse.Data = readMessageRequest;
                if (result)
                    clsResponse.StatusCode = (int)APIResponseCode.Success;
                else
                    clsResponse.StatusCode = (int)APIResponseCode.Failure;
                return Ok(clsResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Poll")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Message", "{UserName} Read Save Poll Answer")]
		public async Task<ActionResult<APIResponse>> SavePollAnswer([FromBody] PollResultsDTO pollResult)
        {
            PollResults _pollResult = _mapper.Map<PollResults>(pollResult);
            var _poll = await _messageService.SavePoll(_pollResult);

            PollDTO pollDTO = _mapper.Map<PollDTO>(_poll);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = pollDTO;
            return Ok(apiResponse);
        }
        [HttpPost]
        [AuthorizeOIDC]
        [Route("ArticleComment")]
     //   [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Message", "{UserName} Read Save Poll Answer")]
        public async Task<ActionResult<APIResponse>> AddArticleComment([FromBody] ArticleCommentDTO articleCommentDTO)
        {
            ArticleComments article = _mapper.Map<ArticleComments>(articleCommentDTO);
            var _poll = await _messageService.AddArticleComment(article);

            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = articleCommentDTO;
            return Ok(apiResponse);
        }


        [AuthorizeOIDC]
        [HttpGet]
        [Route("Comment/List")]
        [ActionLog("Feed", "{UserName} CommentList")]

        [SwaggerOperation(Summary = "Reviewed")]
        public async Task<ActionResult<APIResponse>> CommentList(long ArticleId, long CustomerId, long pageNumber, long pageSize)
        {
            var comments = await _messageService.CommentList(ArticleId, CustomerId, pageNumber,pageSize);
            APIResponse objResponse = new APIResponse();
            objResponse.StatusCode = (int)APIResponseCode.Success;
            objResponse.Data = comments;
            return Ok(objResponse);
        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("PostArticle")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Message", "{UserName} Post Article")]

        public async Task<ActionResult<APIResponse>> SaveArticle(PostArticleDTO newsFeedsDTO)
        {
            try
            {
                List<string> imageList = new List<string>();
                long loggedInUserId = _common.CurrentUser().Id;
                var communityName = _common.CurrentUser().PrimaryCommunity.CommunityName;

                if (newsFeedsDTO.Type.ToLower() == "all")
                {
                    newsFeedsDTO.IsGroup = 0;
                    newsFeedsDTO.ReferenceId = 0;
                }
                else
                {
                    newsFeedsDTO.IsGroup = 1;
                }
                newsFeedsDTO.CommunityName = communityName;
                newsFeedsDTO.CustomerId = loggedInUserId;
                newsFeedsDTO.ReferenceTypeId = 0;
                newsFeedsDTO.IsArchived = false;
                NewsFeeds article = _mapper.Map<NewsFeeds>(newsFeedsDTO);

                if (newsFeedsDTO.ArticleMedia != null && newsFeedsDTO.ArticleMedia.Count > 0)
                {
                    article.ArticleMedia = newsFeedsDTO.ArticleMedia
                        .Select(mediaDto => new ArticleMedia
                        {

                            Media = mediaDto.Media,
                            MediaType = mediaDto.MediaType,
                            filename = mediaDto.filename,
                            CreatedBy = loggedInUserId,
                            ModifiedBy = loggedInUserId,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            IsActive = true

                        }).ToList();
                }
                var result = await _messageService.SendNewFeeds(article);

                APIResponse apiResponse = new APIResponse();
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = article;
                return Ok(apiResponse);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);  
            }
            

        }



        [HttpPost]
        [AuthorizeOIDC]
        [Route("GroupList")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Message", "{UserName} Community Group Dropdown list")]
        public async Task<ActionResult<APIResponse>> DropDownGroupList([FromBody] GroupCommunityId CommunityId)
        {
            var key = await _communityService.GetCommunityGroupList(CommunityId.CommunityId);
            var DropDownGroupList = (key?.Select(u => _mapper.Map<Groups>(u)).ToList());
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = DropDownGroupList;
            return Ok(apiResponse);
        }



        [HttpPost]
        [AuthorizeOIDC]
        [Route("SendBroadcast")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Message", "{UserName} Send Broadcast Message")]
        public async Task<ActionResult<APIResponse>> SendBroadcastMessage(PostBroadcast broadcastDTO)
        {
            var logger = LogManager.GetLogger("database");
            logger.Info("loggerTest broadcast");

            try
            {
                //var communityName = _common.CurrentUser().PrimaryCommunity.CommunityName;
                //var UserId = _common.CurrentUser().Id;
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
                    return BadRequest();
                }

                // Set other broadcastDTO properties
                if (broadcastDTO.Type.ToLower() == "all")
                {
                    broadcastDTO.IsGroup = 0;
                    broadcastDTO.ReferenceId = 0;
                }
                else
                {
                    broadcastDTO.IsGroup = 1;
                }

                //broadcastDTO.CommunityName = broadcastDTO.CommunityName;
                //broadcastDTO.CustomerId = broadcastDTO.CustomerId;
                //broadcastDTO.CommunityId = _common.CurrentUser().PrimaryCommunity.Id;

                // Map to Broadcast entity and save
                Broadcast broadcastMessage = _mapper.Map<Broadcast>(broadcastDTO);
                var result = await _messageService.SaveBroadcastMessage(broadcastMessage);
                logger.Info("Inside SaveBroadcastMessage");

                APIResponse apiResponse = new APIResponse();
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = broadcastMessage;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                logger.Info("Error sending broadcast: " + ex.Message);
                return BadRequest();
            }
        }

    }
}
