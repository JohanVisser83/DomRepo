using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.User;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.Storefront;
using Circular.Services.User;
using CircularWeb.Business;
using CircularWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using OpenIddict.Client;
using System.Data;
using System.Globalization;
using System.Net.Http.Headers;

namespace CircularWeb.Controllers
{
    [Authorize]
    public class StorefrontController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly ICustomerService _customerService;
        private readonly IGlobal _global;
        Customers customer;
        private IServiceProvider _provider;
        private string OIDCUrl;

        public StorefrontModel StorefrontModel = new StorefrontModel();
        private readonly IStorefrontServices _storefrontServices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICommunityService _communityService;
        List<string> UploadImage = new List<string>();
        List<string> uploadedpaths = new List<string>();

        public StorefrontController(IStorefrontServices storefrontServices, IMapper mapper, IHelper Helper, IHttpContextAccessor httpContextAccessor
            , IWebHostEnvironment webHostEnvironment, IConfiguration configuration, ICustomerRepository customerRepository, ICommunityService communityService, ICustomerService customerService, IServiceProvider provider)
        {
            _storefrontServices = storefrontServices ?? throw new ArgumentNullException(nameof(storefrontServices));
            _mapper = mapper;
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));

            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _config = configuration;
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _helper = Helper ?? throw new ArgumentNullException(nameof(Helper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _global = new Global(_httpContextAccessor, _config, customerRepository);
            _communityService = communityService;
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);

        }

        public async Task<IActionResult> Storefront(long StoreID)
        {
            try
            {
                CurrentUser currentUser = _global.GetCurrentUser();
                long CommunityId = currentUser.PrimaryCommunityId;
                long CustomerID = currentUser.Id;
                StorefrontModel.currencyModel.CurrencyCode = _global.currentUser.Currency;
                StorefrontModel.CommunityLogo = currentUser.CustomerInfo.PrimaryCommunity.CommunityLogo;
                StorefrontModel.CommunityFeatures = _communityService.Features(CommunityId,CustomerID).Result.ToList();
                StorefrontModel.IsOwner = currentUser.CustomerInfo.PrimaryCommunity.OwnerCustomerId == CustomerID ? true : false;
                //StorefrontModel.IsSubscriptionActive = currentUser.subs

                string OrderDate = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (StoreID == 0)

                {
                    StorefrontModel.StoreData = await _storefrontServices.DropDownListStore(CommunityId);
                    //StorefrontModel.ListOrderDetails = await _storefrontServices.OrderDetails(OrderDate, CommunityId,StoreID);
                }
                if (StoreID != 0)
                {
                    StorefrontModel.ListProductData = await _storefrontServices.GetAllProductData(StoreID);
                    StorefrontModel.ListCategoryData = await _storefrontServices.GetCategoryDetail(StoreID);
                    //StorefrontModel.ListOrderDetails = await _storefrontServices.OrderDetails(OrderDate, CommunityId, StoreID);
                }
                else
                {
                    if (StorefrontModel.StoreData != null && StorefrontModel.StoreData.Count() > 0)
                    {
                        StorefrontModel.ListProductData = await _storefrontServices.GetAllProductData(StorefrontModel.StoreData.FirstOrDefault().Id);
                        StorefrontModel.ListofSchedule = await _storefrontServices.GetPreOrderList(StorefrontModel.StoreData.FirstOrDefault().Id);
                        StorefrontModel.ListCategoryData = await _storefrontServices.GetCategoryDetail(StorefrontModel.StoreData.FirstOrDefault().Id);
                    }
                    if (StorefrontModel.ListCategoryData.Count() > 0)
                        StorefrontModel.Listofproducts = await _storefrontServices.GetProductDetails(StorefrontModel.StoreData.FirstOrDefault().Id, StorefrontModel.ListCategoryData.FirstOrDefault().Id);
                    else
                        StorefrontModel.Listofproducts = null;
                    if (StorefrontModel.StoreData != null && StorefrontModel.StoreData.Count() > 0)
                    {
                        StorefrontModel.ListOfCart = await _storefrontServices.POSCartList(CustomerID, StorefrontModel.StoreData.FirstOrDefault().Id);
                    }
                }
                if (!StorefrontModel.IsFeatureAvailable("SF-005"))
                    throw new ArgumentNullException("Unauthroized : You dont have permission to access this functionality.");
                else
                    return View(StorefrontModel);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<IActionResult> ClearCart(long StoreId)
        {
            var result = await _storefrontServices.ClearCart(StoreId);
            if (result != null)
                return Json(new { success = true, message = "", data = result });
            else
                return Json(new { success = false, message = "" });

        }

        [HttpPost]
        public async Task<IActionResult> SaveStoreDetail(StorefrontDTO storefrontDTO)
        {
            try
            {
                var file = Request.Form.Files[0];
                storefrontDTO.CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                string UploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString();
                string Uniquefilename = _helper.SaveFile(file, UploadFolder, this.Request);
                storefrontDTO.DisplayImage = Convert.ToString(Uniquefilename);
                CustomerStore customerStore = _mapper.Map<CustomerStore>(storefrontDTO);

                var result = await _storefrontServices.SaveStoreDetail(customerStore);

                if (result > 0)
                {
                    return Json(new { success = true, message = "Store added successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Record not saved successfully!" });
                }




            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }

        }

        [HttpPost]
        public async Task<IActionResult> SaveCategoryDetail(StoreProductCategoryDTO storeProductCategoryDTO)
        {
            try
            {
                var categoryFile = Request.Form.Files[0];
                string UploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString();
                string Uniquefilename = _helper.SaveFile(categoryFile, UploadFolder, this.Request);
                storeProductCategoryDTO.Icon = Convert.ToString(Uniquefilename);
                StoreProductCategory storeProductCategory = _mapper.Map<StoreProductCategory>(storeProductCategoryDTO);
                var result = await _storefrontServices.SaveCategoryDetail(storeProductCategory);

                if (result > 0)
                {
                    return Json(new { success = true, message = "Record Saved successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Record not Saved successfully!" });
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }
        [HttpGet]

        public async Task<ActionResult<StoreProductCategory>?> GetCategoryDetail(long StoreID)
        {

            var CategoryData = await _storefrontServices.GetCategoryDetail(StoreID);
            var GetCategoryData = (CategoryData?.Select(u => _mapper.Map<StoreProductCategoryDTO>(u)).ToList());
            return View();

        }
        [HttpGet]
        public async Task<ActionResult<StorefrontDTO>?> GetStoreName(string Name)
        {
            try
            {
                long CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                var StoreData = await _storefrontServices.GetStoreName(CommunityId, Name);
                var GetStoreData = (StoreData?.Select(u => _mapper.Map<StorefrontDTO>(u)).ToList());
                return Json(new { Data = GetStoreData, success = true, Message = "get data" });
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult<StorefrontDTO>?> GetCategoryName(string Name, long StoreId)
        {
            try
            {

                var CategoryData = await _storefrontServices.GetCategoryName(Name, StoreId);
                var GetCategoryData = (CategoryData?.Select(u => _mapper.Map<StoreProductCategoryDTO>(u)).ToList());
                return Json(new { Data = GetCategoryData, success = true, Message = "get data" });
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<StoreProductCategoryDTO>?> DropDownListCategory(long StoreID)
        {
            try
            {
                var CategoryData = await _storefrontServices.DropDownListCategory(StoreID);
                var GetCategoryData = (CategoryData?.Select(u => _mapper.Map<StoreProductCategoryDTO>(u)).ToList());
                return Json(new { Data = GetCategoryData, success = true, Message = "get data" });


            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult<TransactionsDTO>?> Storewallet(long StoreID)
        {
            try
            {
                var Wallet = await _storefrontServices.Storewallet(StoreID);
                var BallanceWallet = (Wallet?.Select(v => _mapper.Map<TransactionsDTO>(v)).ToList());

                if (BallanceWallet.Count > 0)
                    return Json(new { Data = BallanceWallet, success = true, Message = "getdata" });
                else
                    return Json(new { success = false, message = "" });

            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveProductDetail(ProductsDTO productsDTO)
        {               
                var communityName = _global.currentUser.PrimaryCommunityName;
                string updatedcommunityName = communityName.Replace("-", " ").Replace("@", "").Replace("(", "").Replace(")", "").Replace(",", "").Replace("~", "").Replace("#", "").Replace("$", "").Replace("*", "").Replace("^", "").Replace("!", "").Replace("'", "");
                string UploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString()+ "/" + updatedcommunityName + "/Product" ;
                string _imgPath = "";
                for (int i = 0; i < productsDTO.Images.Count; i++)
                {
                    string imagepath = "";
                    string imagefile = productsDTO.Images[i].filename;
                    string ImgBase64 = productsDTO.Images[i].ImagePath;

                    imagepath = _helper.ConvertBase64toImage(imagefile, ImgBase64, UploadFolder, this.Request);
                    productsDTO.Images[i].ImagePath = Convert.ToString(imagepath);

                    StorefrontModel.currencyModel.CurrencyCode = _global.currentUser.Currency;
                    string _newimgPath = imagepath;
                    _imgPath += _newimgPath + ",";
                }
                var CustomerID = _global.GetCurrentUser().Id;
                productsDTO.CustomerId = CustomerID;              
                var result = await _storefrontServices.SaveProductDetail(productsDTO.CategoryId,productsDTO.ProductName,productsDTO.ProductDesc,productsDTO.ProductUniqueID, _imgPath.Remove(_imgPath.Length - 1), productsDTO.SellingPrice,productsDTO.Quantity,productsDTO.Threshold,productsDTO.CustomerId);
                if (result >= 0)
                {
                    return Json(new { success = true, message = "Product  added successfully" });
                }
                else if (result == -1)
                {
                    return Json(new { success = false, message = "Product already exists!" });
                }
                else
                {
                    return Json(new { success = false, message = "Record Not Saved successfully!" });
                }
           
        }

        [HttpGet]
        public async Task<ActionResult<ProductsDTO>?> GetProductName(string Name, long CategoryID)
        {
            try
            {

                var ProductData = await _storefrontServices.GetProductName(Name, CategoryID);
                var Product = (ProductData?.Select(u => _mapper.Map<ProductsDTO>(u)).ToList());
                return Json(new { Data = Product, success = true, Message = "get data" });
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult<ProductsDTO>?> GetProductID(string Name, string ProductID)
        {
            try
            {
                var ProductData = await _storefrontServices.GetProductID(Name, ProductID);
                var Product = (ProductData?.Select(u => _mapper.Map<ProductsDTO>(u)).ToList());
                return Json(new { Data = Product, success = true, Message = "get data" });

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult<StorefrontScheduleDTO>?> EditScheduleData(long ID)
        {
            try
            {

                var ScheduleData = await _storefrontServices.EditPreorder(ID);
                var Data = (ScheduleData?.Select(u => _mapper.Map<StorefrontSchedule>(u)).ToList());
                return Json(new { Data = Data, success = true, Message = "get data" });
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }
        //[HttpGet]
        //public async Task<ActionResult<stockInventoryDTO>?> GetProductByID(long id)
        //{
        //    try
        //    {

        //        var ProducteditData = await _storefrontServices.GetProductByID(id);
        //        var EditProduct = (ProducteditData?.Select(u => _mapper.Map<stockInventory>(u)).ToList());
        //        return Json(new { Data = EditProduct, success = true, Message = "get data" });
        //    }
        //    catch (Exception ex)
        //    {

        //        return Json(ex.Message);
        //    }
        //}
        public async Task<IActionResult> GetProductByID(long id)
        {
            var ProducteditData = await _storefrontServices.GetProductByID(id);
            var EditProduct = (ProducteditData?.Select(u => _mapper.Map<stockInventory>(u)).ToList());
            return Json(new { Data = EditProduct, success = true, Message = "get data" });
        }
        [HttpGet]
        public async Task<ActionResult<stockInventoryDTO>?> StockListbind(long StoreID)
        {
            try
            {
                var ProducteditData = await _storefrontServices.GetAllProductData(StoreID);
                var StockProduct = (ProducteditData?.Select(u => _mapper.Map<stockInventory>(u)).ToList());
                return Json(new { Data = StockProduct, success = true, Message = "get data" });
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }

        }
        [HttpGet]
        public async Task<ActionResult<StoreProductCategoryDTO>?> POSCategoryBind(long StoreID)
        {
            try
            {
                var POSCategoryData = await _storefrontServices.GetCategoryDetail(StoreID);
                var POSCategory = (POSCategoryData?.Select(u => _mapper.Map<StoreProductCategory>(u)).ToList());
                return Json(new { Data = POSCategory, success = true, Message = "get data" });
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }

        }
        [HttpGet]
        public async Task<ActionResult<ProductsDTO>?> POSProductBind(long StoreID, long CategoryID)
        {
            try
            {
                var POSProductData = await _storefrontServices.GetProductDetails(StoreID, CategoryID);
                var POSProduct = (POSProductData?.Select(u => _mapper.Map<Products>(u)).ToList());
                return Json(new { Data = POSProduct, success = true, Message = "get data" });

            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }

        }

        [HttpGet]
        public async Task<ActionResult<AddCartDTO>?> POSItemCartBind(long StoreID)
        {
            try
            {
                var CustomerID = _global.GetCurrentUser().Id;

                var POSCartData = await _storefrontServices.POSCartList(CustomerID, StoreID);
                var POSCart = (POSCartData?.Select(u => _mapper.Map<AddCartDTO>(u)).ToList());
                return Json(new { Data = POSCart, Success = true, Message = "get data" });

            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }

        //[HttpGet]

        //public async void GetProductExcel(long id)
        //{
        //    try
        //    {
        //        var ProductexcelData = await _storefrontServices.GetProductExcel(id);
        //        var ExcelProductList = (ProductexcelData?.Select(u => _mapper.Map<ProductsDTO>(u)).ToList());
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
        public async Task<ActionResult> DeleteStockAndProduct(long ProductId)
        {
            long stock = await _storefrontServices.DeleteStockAndProductAsync(ProductId);
            if (stock > 0)
                return Json(new { success = true, message = "Poll Deleted Successfully!" });
            else
                return Json(new { success = false, message = "Poll Deleted not Successfully!" });
        }

        public async Task<ActionResult<StorefrontScheduleDTO>?> UpdateTimeSlots(StorefrontSchedule storefrontSchedule)
        {

            try
            {
                var CustomerID = _global.GetCurrentUser().Id;

                StorefrontSchedule Schedule = _mapper.Map<StorefrontSchedule>(storefrontSchedule);
                var result = await _storefrontServices.UpdateTimeSlotes(Schedule);

                if (result > 0)
                {
                    return Json(new { success = true, message = "Record Saved successfully" });
                }
                else
                {
                    return Json(new { success = false, messege = "Error..." });
                }


            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public async Task<ActionResult<StorefrontScheduleDTO>?> SavePreOrderSlots(StorefrontScheduleDTO storefrontScheduleDTO)
        {
            try
            {
                long CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                storefrontScheduleDTO.CommunityId = CommunityId;
                var CustomerID = _global.GetCurrentUser().Id;

                StorefrontSchedule Slots = _mapper.Map<StorefrontSchedule>(storefrontScheduleDTO);
                var result = await _storefrontServices.AddPreOrderSlots(Slots);
                if (result > 0)
                {
                    return Json(new { Data = result, success = true, message = "Record Saved successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Record Not Saved successfully!" });
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }

        }


        public async Task<ActionResult<stockInventoryDTO>?> SaveProductInventory(stockInventoryDTO stockInventoryDTO)
        {
            try
            {
                stockInventory StockInvetory = _mapper.Map<stockInventory>(stockInventoryDTO);

                var result = await _storefrontServices.SaveProductInventory(StockInvetory);
                if (result > 0)
                {
                    return Json(new { success = true, message = "Record Saved successfully" });
                }
                else
                {
                    return Json(new { success = false, messege = "Error..." });
                }
            }
            catch (Exception)
            {

                throw;
            }

        }




        public async Task<ActionResult<AddCartDTO>?> SaveItemCart(AddCartDTO addCartDTO)
        {
            try
            {
                var CustomerID = _global.GetCurrentUser().Id;
                addCartDTO.CustomerId = CustomerID;
                AddCart addCart = _mapper.Map<AddCart>(addCartDTO);
                var result = await _storefrontServices.SaveIteminCart(addCart);
                if (result > 0)
                {
                    return Json(new { success = true, message = "Record Saved successfully" });
                }
                else
                {
                    return Json(new { success = false, messege = "Error..." });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<ActionResult<OrderDTO>?> BuyerListbind(long StoreID, string Startdate , string Enddate)
        {
            try
            {
                if (string.IsNullOrEmpty(Startdate) || string.IsNullOrWhiteSpace(Startdate) && (string.IsNullOrEmpty(Enddate) || string.IsNullOrWhiteSpace(Enddate)))
                {
                    Startdate = DateTime.Now.ToString("MM/dd/yyyy");
                    Enddate = DateTime.Now.ToString("MM/dd/yyyy");
                }

                long CommunityId = _global.GetCurrentUser().PrimaryCommunityId;


                var BuyyerDataList = await _storefrontServices.OrderDetails(Startdate, CommunityId, StoreID,Enddate);
                var BuyerList = (BuyyerDataList?.Select(u => _mapper.Map<Order>(u)).ToList());
                if (BuyerList.Count() > 0 && BuyerList != null)
                    return Json(new { data = BuyerList, success = true, Message = "get data" });
                else
                    return Json(new { success = false, Message = "no data found" });
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }

        }
        [HttpGet]
        public async Task<ActionResult<OrderDTO>?> BuyerProductBind(long BuyerId)
        {
            try
            {
                long CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                var BuyyerProductList = await _storefrontServices.BuyerProductList(BuyerId, CommunityId);
                var BuyerList = (BuyyerProductList?.Select(u => _mapper.Map<Order>(u)).ToList());
                return Json(new { Data = BuyerList, success = true, Message = "get data" });
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }

        public async Task<IActionResult> Refundamount( long UserId, decimal Amount, string RefundNote, long StoreId)
        {
            try
            {
                long CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
             var Currency = _global.currentUser.Currency;
                //CurrentUser currentUser = _global.GetCurrentUser();
                var  TransactionFrom = _global.GetCurrentUser().Id;

                var result = await _storefrontServices.RefundAmount(CommunityId,UserId, Amount,RefundNote,Currency,StoreId, TransactionFrom);
                if (result != null)
                {
                    return Json(new { success = true, message = "Record Saved successfully" });
                }
                else
                {
                    return Json(new { success = false, messege = "Error..." });
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }

        public async Task<ActionResult<TransactionsDTO>?> CheckoutPayment(TransactionsDTO transactionsDTO)
        {
            try
            {

                transactionsDTO.CommunityId = _global.currentUser.PrimaryCommunityId;
                var CustomerID = _global.GetCurrentUser().Id;
                transactionsDTO.CustomerId = CustomerID;
                // transactionsDTO.CommunityId = CommunityId;

                Transactions transaction = _mapper.Map<Transactions>(transactionsDTO);
                var result = await _storefrontServices.CheckoutPayment(transaction);
                if (result > 0)
                {
                    return Json(new { success = true, message = "Record Saved successfully" });
                }
                else
                {
                    return Json(new { success = false, messege = "Error..." });
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }

        public async Task<ActionResult<AddCartDTO>?> GetPOSCartDetails(long StoreID)
        {
            try
            {
                var CustomerID = _global.GetCurrentUser().Id;

                var CartData = await _storefrontServices.GetAllCartDetails(CustomerID, StoreID);
                var PosCart = (CartData?.Select(u => _mapper.Map<AddCartDTO>(u)).ToList());
                if (PosCart != null && PosCart.Count() > 0)
                    return Json(new { success = true, message = "Data fetched successfully!", data = PosCart });
                else
                    return Json(new { success = false, message = "No Data Found!", data = "" });

            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }

        }

        public async Task<IActionResult> CreateOrderQr1(OrderDTO vec)
        {


            vec.CustomerId = _global.GetCurrentUser().Id;

            Order orders = _mapper.Map<Order>(vec);
            Order order = await _storefrontServices.CreateOrderQr1(orders);
            return Json(new { success = true, data = order });
        }

        //[HttpPost]
        //public async Task<ActionResult<OrderDTO>?> CreateOrderQr(OrderDTO orderDTO)
        //{
        //    try
        //    {
        //        OrderDTO generateQRCode = new OrderDTO();

        //        GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(orderDTO.OrderId.ToString());

        //        //barcode.SetMargins(400);
        //        barcode.ChangeBarCodeColor(System.Drawing.Color.Black);
        //        string path = Path.Combine(_webHostEnvironment.WebRootPath, "PaymentQRCode");
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        string QrCode = GetUniqueFileName(orderDTO.OrderId.ToString());
        //        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "PaymentQRCode/" + QrCode + ".png");
        //        barcode.SaveAsPng(filePath);
        //        string fileName = Path.GetFileName(filePath);
        //        string imageUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/PaymentQRCode/" + fileName;
        //        ViewBag.QrCodeUri = imageUrl;
        //        orderDTO.QRCode = imageUrl;
        //        return Json(new { response = imageUrl, success = true });
        //    }
        //    catch (Exception ex)
        //    {

        //        return Json(ex.Message);
        //    }
        //}
        //private string GetUniqueFileName(string fileName)
        //{
        //    fileName = Path.GetFileName(fileName);
        //    return Path.GetFileNameWithoutExtension(fileName)
        //              + "_"
        //              + Guid.NewGuid().ToString().Substring(0, 4)
        //              + Path.GetExtension(fileName);
        //}

        [HttpGet]
        public async Task<ActionResult<OrderDTO>?> StorePreorderCustomer(string Mobile, long StoreId)
        {
            try

            {

                var Wallet = await _storefrontServices.StorePreorderCustomer(Mobile, StoreId);
                var BallanceHistory = (Wallet?.Select(v => _mapper.Map<OrderDTO>(v)).ToList());
                return Json(new { Data = BallanceHistory, success = true, Message = "getdata" });
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<OrderDTO>?> StoreCashRagister(long StoreId, string Startdate, string EndDate)
        {
            try

            {
                long CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                var StoreRagister = await _storefrontServices.StoreCashRagister(CommunityId, StoreId, Startdate, EndDate);
                var BallanceHistory = (StoreRagister?.Select(v => _mapper.Map<OrderDTO>(v)).ToList());
                return Json(new { Data = StoreRagister, success = true, Message = "getdata" });
            }
            catch (Exception ex)
            {

                return Json(ex.Message);
            }
        }





        public async Task<ActionResult> UpdateStockProcductInventory(stockInventoryDTO stockInventoryDTO)
        {
            try
            {
                long communityId = _global.currentUser.PrimaryCommunityId;
                //string CoverImage = string.Empty;
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    string UploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString();
                    SavemultipleFilesupdate(file, UploadFolder, this.Request);
                    var data = UploadImage;
                    if (data.Count() > 0)
                    {
                        stockInventoryDTO.ProductImage = data[0];
                    }
                }
                else
                {
                    if (stockInventoryDTO.ProductImage == "undefined")
                    {
                        var ProducteditData = await _storefrontServices.GetProductByID(stockInventoryDTO.Id);
                        stockInventoryDTO.ProductImage = ProducteditData.FirstOrDefault().ProductImage;
                    }
                }

                stockInventoryDTO.ProductImage = Convert.ToString(stockInventoryDTO.ProductImage);
                stockInventory StockInventory = _mapper.Map<stockInventory>(stockInventoryDTO);
                var result = await _storefrontServices.UpdateProductInventory(StockInventory);
                if (result != null)
                {
                    return Json(new { success = true, message = "Your campaign has been updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Data Not Updated Successfully!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the campaign" });
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


                path = Path.Combine(filesPath, uniqueFileName);
                file.CopyToAsync(new FileStream(path, FileMode.Create));
                returnfilepath = Path.Combine(url, uniqueFileName);
                UploadImage.Add(returnfilepath);
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
        
        public async Task<IActionResult> DeleteStore(long Id)
        {
            var result = await _storefrontServices.DeleteStore(Id);
            if (result > 0)
                return Json(new { success = true, message = "Store Deleted Successfully" });
            else
                return Json(new { success = false, message = "Store Not Deleted Successfully" });
        }
        public async Task<IActionResult> GetCustomerProductDetails(long StoreId, string MobileNo)
        {
            var result = await _storefrontServices.GetCustomerProductDetails(MobileNo, StoreId);
            if (result != null)
                return Json(new { success = true, data = result, message = "Store Deleted Successfully" });
            else
                return Json(new { success = false, message = "Store Not Deleted Successfully" });
        }

        public async Task<IActionResult> UpdateOrderMarkAsCollected(long OrderId)
        {
            var result = await _storefrontServices.UpdateOrderMarkAsCollected(OrderId);
            if (result == true)
                return Json(new { success = true, data = result, message = "Mark As Collected Successfully" });
            else
                return Json(new { success = false, message = "" });
        }
        public async Task<ActionResult> SendOtpForWithdrawal()
        {
            var lstOTP = _helper.GenerateRandomNumber(int.Parse(_config["OTP:Length"]), _config["OTP:MasterOTP"],
               bool.Parse(_config["OTP:IsMasterOTPEnabled"]), bool.Parse(_config["OTP:IsAlphaNumeric"]));
            var userName = _global.GetCurrentUser().MobileNumber;
            var response = await SaveOTPAsync(userName, lstOTP.ToString(), true);
            string data = response.ToJson();
            APIResponse objResponse = JsonConvert.DeserializeObject<APIResponse>(data);
            if (objResponse.StatusCode == 200)
            {
                var sentOTPMail = await _customerService.SendOTPMail(userName, lstOTP.ToString());
                if (sentOTPMail == "True")
                    return Json(new { success = true, message = "Item Deleted Successfully" });
                else
                    return Json(new { success = false, message = "Something went wrong" });
            }
            else
            {
                return Json(new { success = false, message = "Something went wrong" });
            }
        }
        public async Task<ActionResult> SaveOTPAsync(string userName, string otp, bool signupFlow)
        {
            var OIDCClient = _provider.GetRequiredService<OpenIddictClientService>();
            var clientDetails = await OIDCClient.GetClientRegistrationAsync(new Uri(OIDCUrl));
            if (clientDetails != null)
            {
                using var client = _provider.GetRequiredService<HttpClient>();
                using var request = new HttpRequestMessage(HttpMethod.Post, OIDCUrl + "connect/save-otp");
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>{
                    {"client_id", clientDetails.ClientId},
                    {"client_secret", clientDetails.ClientSecret},
                    {"username", userName},
                    {"otp", otp}
                });

                var response = await client.SendAsync(request);
                var result = "";
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                return BadRequest();
            }
            else
            {
                return Unauthorized();
            }
        }
        public async Task<ActionResult> VerifyPassword(string? otp, bool isLoginFlow = false)
        {
            try
            {
                var UserName = _global.GetCurrentUser().MobileNumber;
                var Isloginotp = await GetTokenByOtpAsync(UserName, otp, true, "");
                if (Isloginotp)
                    return Json(new { success = true, message = "OTP Verified Successfully" });
                else
                    return Json(new { success = false, message = "Invaild OTP" });


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Invaild OTP" });
            }
        }
        async Task<bool> GetTokenByOtpAsync(string userName, string otp, bool signupFlow, string CountryCode)
        {
            bool objResponse = false;
            var OIDCClient = _provider.GetRequiredService<OpenIddictClientService>();
            var clientDetails = await OIDCClient.GetClientRegistrationAsync(new Uri(OIDCUrl));
            if (clientDetails != null)
            {
                var client = _provider.GetRequiredService<HttpClient>();
                using var request = new HttpRequestMessage(HttpMethod.Post, OIDCUrl + "connect/otp");
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>{
                    {"client_id", clientDetails.ClientId},
                    {"client_secret", clientDetails.ClientSecret},
                    {"grant_type","password" },
                    {"username", userName },
                    {"password", otp }
                });
                using var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    OpenIDLoginResponse obj = JsonConvert.DeserializeObject<OpenIDLoginResponse>(json);

                    using var clientIdentityResponse = _provider.GetRequiredService<HttpClient>();
                    using var requestIdentityResponse = new HttpRequestMessage(HttpMethod.Get, OIDCUrl + "api/Identity");
                    requestIdentityResponse.Headers.Authorization = new AuthenticationHeaderValue("Bearer", obj.access_token);
                    using var identityResponse = await clientIdentityResponse.SendAsync(requestIdentityResponse);
                    if (identityResponse.IsSuccessStatusCode)
                    {
                        string identityjson = await identityResponse.Content.ReadAsStringAsync();
                        OpenIDIdentityResponse objIdentityResponse = JsonConvert.DeserializeObject<OpenIDIdentityResponse>(identityjson);
                        obj.User_Code = objIdentityResponse.value;
                        // Circular specific logic
                        customer = await GetCustomer(userName, new Guid(obj.User_Code), CountryCode);
                        objResponse = true;
                    }
                    return objResponse;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private async Task<Customers> GetCustomer(string username, Guid usercode, string CountryCode)
        {
            return _customerService.getcustomerByUserId(usercode, true);
        }

        //QR

        public async Task<IActionResult> AttendanceQRCode(DeleteVecDTO vec)
        {
            QR attendance = _storefrontServices.GetAttendanceQRCode(_global.currentUser.PrimaryCommunityId);
            return Json(new { success = true, data = attendance });
        }

        public async Task<IActionResult> DeleteSalesItems(long Id)
        {

            var result = await _storefrontServices.DeleteSalesItemsAsync(Id);
            if (result > 0)
                return Json(new { success = true, message = "Item Deleted Successfully" });
            else
                return Json(new { success = false, message = "Item Not Deleted Successfully" });
        }


        public async Task<IActionResult> StorefrontReconEmail(CashRegisterEmailRecon cashRegisterEmailRecon)
        {
            long CommunityId = _global.GetCurrentUser().PrimaryCommunityId;


            var sentEmail = await _storefrontServices.SentReconEmail(CommunityId, cashRegisterEmailRecon);
            return Json(sentEmail);

        }
        public async Task<ActionResult> StockEdit(stockInventoryDTO stockInventoryDTO)
        {



            stockInventory stockInventory = _mapper.Map<stockInventory>(stockInventoryDTO);
            // stockInventoryDTO.CustomerId = stockInventoryDTO.CustomerId = _global.currentUser.Id;

            var result = await _storefrontServices.StockEdit(stockInventory);
            if (result != null)
                return Json(new { success = true, message = "", data = result });
            else
                return Json(new { success = false, message = "" });

        }
        public async Task<IActionResult> DeletePreOrdering(long Id)
        {
            var result = await _storefrontServices.DeletePreOrderingAsync(Id);
            if (result > 0)
                return Json(new { success = true, message = "Slot Deleted Successfully" });
            else
                return Json(new { success = false, message = "Slot Not Deleted Successfully" });
        }

        public async Task<IActionResult> GetPreOrderList(long StoreId)
        {
            StorefrontModel.ListofSchedule = await _storefrontServices.GetPreOrderList(StoreId);
            return new JsonResult(new { success = true, data = StorefrontModel.ListofSchedule });
        }

        public async Task<ActionResult> StoreUpdate(CustomerStoreDTO customerStoreDTO)
        {
            try
            {
                var file = Request.Form.Files[0];
                customerStoreDTO.CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                string UploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString();
                string Uniquefilename = _helper.SaveFile(file, UploadFolder, this.Request);
                customerStoreDTO.DisplayImage = Convert.ToString(Uniquefilename);
                CustomerStore messagelist = _mapper.Map<CustomerStore>(customerStoreDTO);
                var result = await _storefrontServices.UpdateStore(messagelist);
                // var result = 1;
                if (result > 0)
                    return Json(new { success = true, message = "" });
                else
                    return Json(new { success = false, message = "" });
            }
            catch (Exception ex)
            {
                // throw ex;
                return Json(new { success = false, message = "Oops. Something went wrong!" });
            }
        }

        public async Task<ActionResult> DleteMultiImageStore(long Id)
        {
            var result = await _storefrontServices.DleteMultiImageStore(Id);
            if (result > 0)
                return Json(new { success = true, message = "Deleted Successfully!" });
            else
                return Json(new { success = false, message = "Not Successfully!" });
        }
        [HttpPost]
        public async Task<ActionResult> StoreUploadImageAndFile6(StoreProductImagesDTO obj)
        {
            long Community = _global.currentUser.PrimaryCommunityId;
            string CoverImage = string.Empty;
            var file = Request.Form.Files[0];
            string UploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString();
            SavemultipleFiles(file, UploadFolder, this.Request);
            var data = uploadedpaths;
            if (data.Count() > 0)
            {
                CoverImage = data[0];
            }
            obj.ImagePath = Convert.ToString(CoverImage);
            var result = await _storefrontServices.StoreUploadImageAndFile6( obj.ProductId, obj.ImagePath);
            if (result != null)
                return Json(new { success = true, message = "Your campaign has been updated successfully" });
            else
                return Json(new { success = false, message = "Data Not Updated Successfully!" });

        }
        public void SavemultipleFiles(IFormFile file, string uploadFolder, HttpRequest request)
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




    }
}


