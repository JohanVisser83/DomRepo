using AutoMapper;
using Circular.Services.User;
using CircularWeb.Models;
using Microsoft.AspNetCore.Mvc;



namespace CircularWeb.Controllers
{
    public class Ae444yux3Controller : Controller
    {
        private ICustomerService _customerService;
        private readonly IMapper _mapper;
        public CommunityModel communityModel = new CommunityModel();
        public Ae444yux3Controller(ICustomerService customerService,IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }

        public async Task<IActionResult> Ae444yux3()
        {
          
            communityModel.ActiveOTPDetails =  await  _customerService.GetOTPUser();
            
         
            return View(communityModel);
        }

    }
}
