using AutoMapper;
using Circular.Core.DTOs;
using Circular.Filters;
using Circular.Framework.Logger;
using Circular.Services.Account;
using Circular.Services.User;
using Microsoft.AspNetCore.Mvc;


namespace Circular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {

        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger; 
        private readonly IAccountServices _accountServices;
        public AccountController(IMapper mapper,ILoggerManager logger, IAccountServices accountServices,ICustomerService customerService)
        {
            _mapper = mapper;
            _accountServices=accountServices;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

       
        [HttpGet]
        [AuthorizeOIDC]
        [Route("List")]
		[ActionLog("Account", "{UserName} Requested Account List")]
		public async Task<ActionResult<APIResponse>> GetAccountList(long CustomerId,long CommunityId, int? IsUpcomingOrPassed,long? CollectionId,int PageSize,int PageNumber)
        {
            var accountList =  _accountServices.GetAccounts(CustomerId,CommunityId, (int)IsUpcomingOrPassed,CollectionId,PageSize, PageNumber);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = accountList;
            return Ok(apiResponse);
        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Pay")]
		[ActionLog("Account", "{UserName} Requested Pay")]

		public async Task<ActionResult<APIResponse>> Pay(PayAccountDTO payAccount)
        {
            var pay = _accountServices.Pay(payAccount.CollectionReqId,payAccount.PayForUserId,payAccount.Amount,payAccount.LoggedInUserId,payAccount.Currency);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode=(int)APIResponseCode.Success;
            apiResponse.Data = pay.Result; 
            return Ok(apiResponse);

        }

    }



    
    }

