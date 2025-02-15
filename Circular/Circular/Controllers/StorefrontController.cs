using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Filters;
using Circular.Services.Planners;
using Circular.Services.Storefront;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Drawing.Printing;
using static Azure.Core.HttpHeader;

namespace Circular.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorefrontController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStorefrontServices _storefrontServices;
        private readonly ICommon _common;
        public StorefrontController(IMapper mapper, IStorefrontServices storefrontServices,ICommon common)
        {
            _mapper = mapper;
            _common = common;
            _storefrontServices = storefrontServices;

        }
        [HttpGet]
        [AuthorizeOIDC]
        [Route("Stores")]
		[ActionLog("Storefront", "{UserName}  Requested Storefront store")]
		public async Task<ActionResult<APIResponse>> GetStores(long CommunityId, int PageSize, int PageNumber)
        {
			IEnumerable<GetStore> order = await _storefrontServices.Storefrontstore(CommunityId, PageSize, PageNumber);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = order;
            return Ok(apiResponse);

        }

		
		[HttpGet]
        [AuthorizeOIDC]
        [Route("Category")]
		[ActionLog("Storefront", "{UserName}  Requested Storefront Category")]
		public async Task<IActionResult> StoreGetCategory(long StoreId, long CommunityId, int PageSize, int PageNumber)
        {
            var Category = await _storefrontServices.GetStoreCategory(StoreId, CommunityId, PageSize, PageNumber);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = Category;
            return Ok(apiResponse);

        }


        [HttpGet]
        [AuthorizeOIDC]
        [Route("Products")]
		[ActionLog("Storefront", "{UserName}  Requested Storefront Products")]
		public async Task<IActionResult> GetStoreProducts(int PageSize, int PageNumber, long CategoryId, long ProductId)
        {
            var Products = await _storefrontServices.GetStoreProducts(CategoryId, ProductId, PageSize, PageNumber);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = Products;
            return Ok(apiResponse);


        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Cart")]
		[ActionLog("Storefront", "{UserName}  Add to Cart")]
		public async Task<IActionResult> AddToCart(AddCartDTO cart)
        {
            var Cart = await _storefrontServices.AddToCart(cart.ProductId ?? 0, cart.Quantity ?? 0, cart.Amount ?? 0, cart.StoreId ?? 0, cart.CustomerId ?? 0);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = Cart;
            return Ok(apiResponse);

        }


        [HttpGet]
        [Route("CartItems")]
        public async Task<IActionResult> GetCartItems(long loggedinuser, long StoreId)
        {
            var CartList = await _storefrontServices.CartList(loggedinuser, StoreId);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = CartList;
            return Ok(apiResponse);

        }


        //  [HttpPost]
        ////  [AuthorizeOIDC]
        //  [Route("Order")]
        //  public async Task<IActionResult> PlaceOrder(long ProductId, long Quantity, decimal Amount, long Storeid, long loggedinuser)
        //  {
        //      var Cart = await _storefrontServices.AddToCart(ProductId, Quantity, Amount, Storeid, loggedinuser);
        //      APIResponse apiResponse = new APIResponse();
        //      apiResponse.StatusCode = (int)APIResponseCode.Success;
        //      apiResponse.Data = Cart;
        //      return Ok(apiResponse);

        //  }



        [HttpPost]
        [AuthorizeOIDC]
        [Route("CheckOut")]
		[ActionLog("Storefront", "{UserName}  Requested CheckOut")]
		public async Task<IActionResult> CheckOut(CheckOutDTO checkOut)
        {
            var Cart = await _storefrontServices.CheckOut(checkOut.Storeid ?? 0, checkOut.TotalAmount ?? 0, checkOut.noofitems ?? 0, checkOut.OrderForId ?? 0, checkOut.loggedinUser ?? 0, checkOut.PromoCode);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = Cart;
            return Ok(apiResponse);

        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Update")]
		[ActionLog("Storefront", "{UserName}  Update Cart")]
		public async Task<IActionResult> UpdateCart(UpdateCartDTO updatecart)
        {
            var Cart = await _storefrontServices.UpdateCart(updatecart.ProductId ?? 0, updatecart.Quantity ?? 0, updatecart.Amount ?? 0, updatecart.Storeid ?? 0, updatecart.loggedinUser ?? 0);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = Cart;
            return Ok(apiResponse);

        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Orders")]
		[ActionLog("Storefront", "{UserName} Orders")]
		public async Task<IActionResult> Orders(collectedOrderDTO collectedOrder)
        {
            var Cart = await _storefrontServices.CollectedPendingOrder(collectedOrder.loggedinuser ?? 0, collectedOrder.Collected ?? 0,collectedOrder.OrderId??0);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = Cart;
            return Ok(apiResponse);
        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Collect")]
		[ActionLog("Storefront", "{UserName} Collect Order")]
		public async Task<IActionResult> CollectOrder(collectDTO collectOrder)
        {
            try
            {
                Order order = new Order();
                order.Id = collectOrder.OrderId;
                order.IsCollected = collectOrder.IsCollected ?? false;
                order.ModifiedBy = collectOrder.CollectedBy;

                var store_sentEmailCollect = await _storefrontServices.SendCollectEmail((long) order.CommunityId);
                var Cart = await _storefrontServices.CollectOrder(order);

                

                APIResponse apiResponse = new APIResponse();
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = Cart;
                return Ok(apiResponse);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        //[HttpPost]
        //[AuthorizeOIDC]
        //[Route("Collect")]
        //public async Task<IActionResult> CollectOrder(collectDTO collectOrder)
        //{
        //    Order order = new Order();
        //    order.Id = collectOrder.OrderId;
        //    order.IsCollected = collectOrder.IsCollected ?? false;
        //    order.ModifiedBy = collectOrder.CollectedBy;
        //    var Cart = await _storefrontServices.CollectOrder(order);
        //    APIResponse apiResponse = new APIResponse();
        //    apiResponse.StatusCode = (int)APIResponseCode.Success;
        //    apiResponse.Data = Cart;
        //    return Ok(apiResponse);
        //}

        [HttpPost]
        [Route("Email")]
        [AuthorizeOIDC]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Storefront", "{UserName} Order Email")]
		public async Task<ActionResult> OrderEmail(EmailOrderDTO emailOrderDTO)
        {
            try
            {
                APIResponse apiResponse = new APIResponse();
                if (await _storefrontServices.EmailOrder(emailOrderDTO, _common.CurrentUser()))
                    apiResponse.StatusCode = (int)APIResponseCode.Success;
                else
                    apiResponse.StatusCode = (int)APIResponseCode.Success;
                return Ok(apiResponse);
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        


    }
}
