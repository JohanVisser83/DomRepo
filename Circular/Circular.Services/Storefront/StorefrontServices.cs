using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.Community;
using Circular.Data.Repositories.Storefront;
using Circular.Framework.Middleware.Emailer;
using Circular.Services.Email;
using MailKit.Search;
using RepoDb.Enumerations;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace Circular.Services.Storefront
{
    public class StorefrontServices : IStorefrontServices
    {
        private readonly IStorefrontRepository _storefrontRepository;
        private readonly IMailService _mailService;
        public StorefrontServices(IStorefrontRepository StoreRepository, IMailService mailService)
        {
            _storefrontRepository = StoreRepository;
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

        #region 
        public async Task<int> SaveStoreDetail(CustomerStore customerStore)
        {

            customerStore.FillDefaultValues();
            return await _storefrontRepository.SaveStoreDetail(customerStore);

        }
        public async Task<IEnumerable<Core.Entity.Order>?> OrderDetails(string Startdate, long CommunityId, long StoreId, string Enddate)
        {
            return await _storefrontRepository.OrderDetails(Startdate, CommunityId, StoreId,Enddate);
        }
        public async Task<IEnumerable<CustomerStore>?> DropDownListStore(long CommunityId)
        {
            return await _storefrontRepository.DropDownListStore(CommunityId);

        }

        public async Task<IEnumerable<Core.Entity.Order>?> StoreCashRagister(long Community, long StoreId, string Startdate, string EndDate)
        {
            return await _storefrontRepository.StoreCashRagister(Community, StoreId, Startdate, EndDate);
        }
        //public async Task<bool> EmailOrder(EmailOrderDTO emailOrderDTO,Customers customer)
        //{
        //    Core.Entity.Order order = CollectedPendingOrder(emailOrderDTO.CustomerId,2, emailOrderDTO.OrderId).Result.FirstOrDefault();
        //    MailRequest mailRequest = new MailRequest();
        //    mailRequest.FromUserId = emailOrderDTO.CustomerId;
        //    mailRequest.To = emailOrderDTO.Email;
        //    mailRequest.ReferenceId = emailOrderDTO.OrderId;

        //    MailSettings mailSettings = _mailService.EmailParameter(MailType.Planner, ref mailRequest);
        //    //Parse the body
        //    string body = mailRequest.Body;
        //    StringBuilder ProductNames = new StringBuilder();
        //    foreach (var item in order.orderDetails)
        //    {
        //        ProductNames.Append("<tr><td style='width: 50%;font-size:12px;font-family: 'Nunito', sans-serif; color: #393939;    padding-bottom: 14px;'>" + item.Quantity + 'x' + item.ProductName + "</td> <td style='width: 50%;text-align:right; font-size:12px; font-family: 'Nunito', sans-serif; color: #393939;  padding-bottom: 14px; text-align:right>" + customer.PrimaryCommunity.currencyCode.ToString() + "" + item.Amount + "</td></tr>");

        //    }
        //    string[] PlaceHolders = {  "$storeName","$Mobile","$AmountPaid","$Date","$Time" ,"$ProductName","$InvoiceNumber"};

        //    string[] Values = { order.StoreName, order.OrderedForName,customer.PrimaryCommunity.currencyCode+Convert.ToString(order.Amount), order.CreatedDate.Date.ToString(), order.CreatedDate.ToShortTimeString(), ProductNames.ToString(), order.Id.ToString() };
        //   if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
        //   {
        //       for (int index = 0; index < PlaceHolders.Length; index++)
        //           body = body.Replace(PlaceHolders[index], Values[index]);
        //   }
        //   mailRequest.Body = body;

        //   return await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);

        //}

        public async Task<bool> EmailOrder(EmailOrderDTO emailOrderDTO, Customers customer)
        {
            Core.Entity.Order order = CollectedPendingOrder(emailOrderDTO.CustomerId, 2, emailOrderDTO.OrderId).Result.FirstOrDefault();
            MailRequest mailRequest = new MailRequest();
            mailRequest.FromUserId = emailOrderDTO.CustomerId;
            mailRequest.To = emailOrderDTO.Email;
            mailRequest.ReferenceId = emailOrderDTO.OrderId;

            MailSettings mailSettings = _mailService.EmailParameter(MailType.OrderPlaced, ref mailRequest);
            //Parse the body
            string body = mailRequest.Body;
            StringBuilder ProductNames = new StringBuilder();
            foreach (var item in order.orderDetails)
            {
                ProductNames.Append("<tr><td style='width: 50%;font-size:12px;font-family: 'Nunito', sans-serif; color: #393939;    padding-bottom: 14px;'>" + item.Quantity + 'x' + item.ProductName + "</td> <td style='width: 50%;text-align:right; font-size:12px; font-family: 'Nunito', sans-serif; color: #393939;  padding-bottom: 14px; text-align:right>" + customer.PrimaryCommunity.currencyCode.ToString() + "" + item.Amount + "</td></tr>");

            }
            string[] PlaceHolders = { "$storeName", "$Mobile", "$AmountPaid", "$Date", "$Time", "$ProductName", "$InvoiceNumber", "$ordernumber", "$username" , "$itemname", "$quantity", "$amount", "$total" };

            string[] Values = { order.StoreName, order.OrderedForName, customer.PrimaryCommunity.currencyCode + Convert.ToString(order.Amount), order.CreatedDate.Date.ToString(), order.CreatedDate.ToShortTimeString(), ProductNames.ToString(), order.Id.ToString(),order.Id.ToString(),customer.CustomerDetails.Name,order.ProductName,order.Quantity.ToString(),order.Amount.ToString(),order.Amount.ToString() };
            if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
            {
                for (int index = 0; index < PlaceHolders.Length; index++)
                    body = body.Replace(PlaceHolders[index], Values[index]);
            }
            mailRequest.Body = body;

            return await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);

        }

        public async Task<IEnumerable<CustomerStore>?> GetStoreName(long CommunityId, string Name)
        {
            return await _storefrontRepository.GetStoreName(CommunityId, Name);

        }

        public async Task<IEnumerable<StoreProductCategory>?> GetCategoryName(string Name, long StoreId)
        {
            return await _storefrontRepository.GetCategoryName(Name, StoreId);

        }

        public async Task<IEnumerable<StoreProductCategory>?> DropDownListCategory(long StoreID)
        {
            return await _storefrontRepository.DropDownListCategory(StoreID);

        }
        public async Task<int> SaveCategoryDetail(StoreProductCategory storeProductCategory)
        {
            storeProductCategory.FillDefaultValues();
            return await _storefrontRepository.SaveCategoryDetail(storeProductCategory);
        }

        public async Task<int> SaveProductDetail(long CategoryId, string ProductName, string ProductDesc, string ProductUniqueID, string ImagePath, decimal SellingPrice, long Quantity, long Threshold, long CustomerId)
        {
            //products.FillDefaultValues();
            return await _storefrontRepository.SaveProductDetail( CategoryId,  ProductName,  ProductDesc,  ProductUniqueID,  ImagePath,  SellingPrice,  Quantity,  Threshold,  CustomerId);
        }
        public async Task<int> SaveProductInventory(stockInventory stockInventory)
        {
            stockInventory.FillDefaultValues();
            return await _storefrontRepository.SaveProductInventory(stockInventory);
        }

        public async Task<bool> UpdateProductInventory(stockInventory stockInventory)
        {
            stockInventory.FillDefaultValues();
            return await _storefrontRepository.UpdateProductInventory(stockInventory);
        }

        public async Task<IEnumerable<Products>?> GetProductName(string Name, long Category)
        {
            return await _storefrontRepository.GetProductName(Name, Category);

        }

        public async Task<IEnumerable<Products>?> GetProductID(string Name, string ProductID)
        {
            return await _storefrontRepository.GetProductID(Name, ProductID);

        }
        //public async Task<IEnumerable</*stockInventory*/>?> GetProductByID(long id)
        //{
        //    return await _storefrontRepository.GetProductByID(id);

        //}
        public async Task<List<stockInventory>> GetProductByID(long id)
        {
            return await _storefrontRepository.GetProductByID(id);
        }
        public async Task<IEnumerable<Products>?> GetProductExcel(long id)
        {
            return await _storefrontRepository.GetProductExcel(id);

        }
        public async Task<IEnumerable<stockInventory>?> GetAllProductData(long StoreID)
        {
            return await _storefrontRepository.GetAllProductData(StoreID);

        }
        public async Task<IEnumerable<StoreProductCategory>?> GetCategoryDetail(long StoreID)
        {
            return await _storefrontRepository.GetCategoryDetail(StoreID);
        }
        public async Task<List<Products>> GetProductDetails(long StoreID, long categoryID)
        {
            return await _storefrontRepository.GetProductDetails(StoreID, categoryID);
        }
      
        public async Task<int> UpdateTimeSlotes(StorefrontSchedule Schedule)
        {
            Schedule.FillDefaultValues();
            return await _storefrontRepository.UpdateTimeSlotes(Schedule);
        }

        public async Task<int> CheckoutPayment(Transactions transactions)
        {
            transactions.FillDefaultValues();

            return await _storefrontRepository.CheckoutPayment(transactions);
        }
        public async Task<IEnumerable<AddCart>?> POSCartList(long loggedinuser, long StoreId)
        {
            return await _storefrontRepository.POSCartList(loggedinuser, StoreId);
        }
        public async Task<int> AddPreOrderSlots(StorefrontSchedule storefrontSchedule)
        {
            storefrontSchedule.FillDefaultValues();
            return await _storefrontRepository.AddPreOrderSlots(storefrontSchedule);
        }
        public async Task<long> DeleteStockAndProductAsync(long ProductId)
        {
            return await _storefrontRepository.DeleteStockAndProductAsync(ProductId);
        }

        public async Task<long> RefundAmount(long communityId, long UserId, decimal Amount, string RefundNote, string Currency, long StoreId, long TransactionFrom)

        {
           
            return await _storefrontRepository.RefundAmount(communityId,UserId,Amount,RefundNote,Currency,StoreId,TransactionFrom);
        }
        public async Task<int> SaveIteminCart(AddCart addCart)
        {
            addCart.FillDefaultValues();
            return await _storefrontRepository.SaveIteminCart(addCart);
        }
        public async Task<IEnumerable<StorefrontSchedule>?> GetPreOrderList(long StoreID)
        {
            return await _storefrontRepository.GetPreOrderList(StoreID);
        }
        public async Task<IEnumerable<Transactions>?> Storewallet(long StoreID)
        {
            return await _storefrontRepository.Storewallet(StoreID);
        }
        public async Task<IEnumerable<Core.Entity.Order>?> StorePreorderCustomer(string Mobile, long StoreId)
        {
            return await _storefrontRepository.StorePreorderCustomer(Mobile, StoreId);
        }
        public async Task<IEnumerable<StorefrontSchedule>?> EditPreorder(long ID)
        {
            return await _storefrontRepository.EditPreorder(ID);
        }

        public async Task<IEnumerable<AddCart>?> GetAllCartDetails(long loggedinuser, long StoreId)
        {
            return await _storefrontRepository.GetAllCartDetails(loggedinuser, StoreId);
        }


        public async Task<IEnumerable<Core.Entity.Order>?> BuyerProductList(long BuyerId, long CommunityId)
        {
            return await _storefrontRepository.BuyerProductList(BuyerId, CommunityId);
        }
        #endregion
        public async Task<Core.Entity.Order> GetOrder(long OrderId)
        {
            return await _storefrontRepository.GetOrder(OrderId);
        }
        public async Task<int> PayOrder(Core.Entity.Order order)
        {
            return await _storefrontRepository.PayOrder(order);
        }

        public async Task<int> CollectOrder(Core.Entity.Order order)
        {

            var store_sentEmailCollect = await SendCollectEmail((long)order.CommunityId);

            var id = await _storefrontRepository.CollectOrder(order);
            MailRequest mailRequest = new MailRequest();
            mailRequest.FromUserId = order.CustomerId;
            mailRequest.ReferenceId = id;
            mailRequest.To = store_sentEmailCollect[0].Email;

            MailSettings mailSettings = _mailService.EmailParameter(MailType.OrderCollected, ref mailRequest);
            string body = mailRequest.Body;
            string[] PlaceHolders = { "$StoreName" };
            string[] Values = { order.StoreName };
            if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
            {
                for (int index = 0; index < PlaceHolders.Length; index++)
                    body = body.Replace(PlaceHolders[index], Values[index]);
            }
            mailRequest.Body = body;
            int a = Convert.ToInt16(await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings));
            return a;


        }
        public async Task<List<CustomerDetails>> SendCollectEmail(long communityId)
        {
            return await _storefrontRepository.SendCollectEmail(communityId);
        }

		// public async Task<int> CollectOrder(Core.Entity.Order order)
		//{
		//   return await _storefrontRepository.CollectOrder(order);
		// }

		public async Task<IEnumerable<GetStore>?> Storefrontstore(long CommunityId, int PageSize, int PageNumber)
        {
            return await _storefrontRepository.Storefrontstore(CommunityId, PageSize, PageNumber);
        }

        public async Task<IEnumerable<dynamic>?> GetStoreCategory(long StoreId, long CommunityId, int PageSize, int PageNumber)
        {
            return await _storefrontRepository.GetStoreCategory(StoreId, CommunityId, PageSize, PageNumber);
        }

        public async Task<IEnumerable<dynamic>?> GetStoreProducts(long CategoryId, long? ProductId, int PageSize, int PageNumber)
        {
            return await _storefrontRepository.GetStoreProducts(CategoryId, ProductId, PageSize, PageNumber);
        }

        public async Task<IEnumerable<dynamic>?> AddToCart(long ProductId, long Quantity, decimal Amount, long Storeid, long loggedinuser)
        {
            return await _storefrontRepository.AddToCart(ProductId, Quantity, Amount, Storeid, loggedinuser);
        }

        public async Task<IEnumerable<dynamic>?> CartList(long loggedinuser, long StoreId)
        {
            return await _storefrontRepository.CartList(loggedinuser, StoreId);
        }

        public async Task<IEnumerable<dynamic>?> CheckOut(long Storeid, decimal TotalAmount, long noofitems, long OrderForId, long loggedinUser, string PromoCode)
        {
            return await _storefrontRepository.CheckOut(Storeid, TotalAmount, noofitems, OrderForId, loggedinUser, PromoCode);
        }

        public async Task<IEnumerable<dynamic>?> UpdateCart(long ProductId, long Quantity, decimal Amount, long Storeid, long loggedinUser)
        {
            return await _storefrontRepository.UpdateCart(ProductId, Quantity, Amount, Storeid, loggedinUser);
        }

        public async Task<IEnumerable<Core.Entity.Order>?> CollectedPendingOrder(long loggedinuser, long Collected, long? OrderId)
        {
            return await _storefrontRepository.CollectedPendingOrder(loggedinuser, Collected, OrderId ?? 0);
        }

        public async Task<IEnumerable<dynamic>> ClearCart(long StoreId)
        {
            return await _storefrontRepository.ClearCart(StoreId);
        }
        public async Task<int> DeleteStore(long id)

        {
            return await _storefrontRepository.DeleteStore(id);
        }
        public async Task<clsStorefrontCustomerDetails> GetCustomerProductDetails(string MobileNo, long StoreId)
        {
            return await _storefrontRepository.GetCustomerProductDetails(MobileNo, StoreId);
        }

        public async Task<bool> UpdateOrderMarkAsCollected(long OrderId)
        {
            return await _storefrontRepository.UpdateOrderMarkAsCollected(OrderId);
        }
        public async Task<CommunityTransportPass> GetQRCodePriceAsync(long id)
        {
            return await _storefrontRepository.GetQRCodePriceAsync(id);
        }
        public QR GetAttendanceQRCode(long id)
        {
            return _storefrontRepository.GetAttendanceQRCode(id);
        }
        public async Task<int> DeleteSalesItemsAsync(long id)
        {

            return await _storefrontRepository.DeleteSalesItemsAsync(id);
        }

        public async Task<int> SentReconEmail(long CommunityId, CashRegisterEmailRecon cashRegisterEmailRecon)
        {

            MailRequest mailRequest = new MailRequest();
            mailRequest.FromUserId = CommunityId;
            mailRequest.ReferenceId = CommunityId;
            mailRequest.To = cashRegisterEmailRecon.Email;
            MailSettings mailSettings = _mailService.EmailParameter(MailType.StorefrontRecon, ref mailRequest);
            string body = mailRequest.Body;
            string[] PlaceHolders = { "$amount", "$Circularpayment", "$Cashsales", "$Receipent" };
            string[] Values = { cashRegisterEmailRecon.TotalSales, cashRegisterEmailRecon.Circularpayment, cashRegisterEmailRecon.CashSales, cashRegisterEmailRecon.TotalReceipent };
            if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
            {
                for (int index = 0; index < PlaceHolders.Length; index++)
                    body = body.Replace(PlaceHolders[index], Values[index]);
            }
            mailRequest.Body = body;
            var result = await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
            if (result)
                return 1;
            else
                return 0;
        }

        public async Task<int> StockEdit(stockInventory stockInventory)
        {
            stockInventory.FillDefaultValues();
            return await _storefrontRepository.StockEdit(stockInventory);
        }
        public async Task<Core.Entity.Order> CreateOrderQr1(Core.Entity.Order order)
        {
            return await _storefrontRepository.CreateOrderQr1(order);
        }
        public async Task<int> DeletePreOrderingAsync(long id)

        {
            return await _storefrontRepository.DeletePreOrderingAsync(id);

        }
        public async Task<int> UpdateStore(CustomerStore item)
        {
            return await _storefrontRepository.UpdateStore(item);
        }
        public async Task<int> DleteMultiImageStore(long Id)
        {
            return await _storefrontRepository.DleteMultiImageStore(Id);
        }
        public async Task<bool> StoreUploadImageAndFile6(long ProductId, string ImagePath)
        {
            return await _storefrontRepository.StoreUploadImageAndFile6(ProductId, ImagePath);
        }
    }
}