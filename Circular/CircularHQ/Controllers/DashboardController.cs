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
using Microsoft.AspNetCore.Mvc;

namespace CircularHQ.Controllers
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
        public HQMessageModel messageModel = new HQMessageModel();

        List<string> uploadedpaths = new List<string>();
        private readonly ICustomerRepository _customerRepository;


        
        public DashboardController(IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, ICommunityService communityService,
            IHelper helper, IHttpContextAccessor httpContextAccessor, IMessageService messageService, ICustomerRepository customerRepository)
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
        } 
            
        [ActionLog("Dashboard", "{0} opened dashboard")]
        [Route("Dashboard/")]
        public async Task<IActionResult> Dashboard()
            {
                CurrentUser cuser = _global.GetCurrentUser();
                long primaryId = cuser.PrimaryCommunityId;
                messageModel.CommunityName = cuser.PrimaryCommunityName;
                messageModel.CommunityLogo = cuser.CustomerInfo.PrimaryCommunity.CommunityLogo;
                messageModel.Communities = await _MessageService.GetCommunitiesAsync();
                messageModel.Customers = await _communityService.GetTotalMembers();


                return View("Dashboard", messageModel);
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

        [ActionLog("Dashboard", "{0} fetched group list.")]
        public async Task<ActionResult> DropDownCommunityUserList(long CommunityId)
        {
            var key = await _communityService.GetCommunityUser(CommunityId);
            var DropDownUserList = (key?.Select(u => _mapper.Map<CustomerDetails>(u)).ToList());
            return Json(new { Data = DropDownUserList, success = true, Message = "get data" });
        }
    }
}
