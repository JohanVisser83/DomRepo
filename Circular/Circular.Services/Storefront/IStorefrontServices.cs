using Circular.Core.DTOs;
using Circular.Core.Entity;
using System.Net.Sockets;

namespace Circular.Services.Storefront
{
   public  interface IStorefrontServices
    {
        #region Store Specific Functions
        public Task<int> SaveStoreDetail(CustomerStore customerStore);

        public Task<IEnumerable<Order>?>OrderDetails(string Startdate ,long CommunityId,long StoreId ,string Enddate);

        

        public Task<IEnumerable<Order>?> StorePreorderCustomer(string Mobile,long StoreId);

        public Task<IEnumerable<Order>?> StoreCashRagister(long Community, long StoreId,string Startdate, string EndDate);

        Task<bool> EmailOrder(EmailOrderDTO emailOrderDTO, Customers customer);


        public Task<IEnumerable<CustomerStore>?> DropDownListStore(long CommunityId);

        public Task<IEnumerable<CustomerStore>?>GetStoreName(long CommunityId,string Name);

        public Task<IEnumerable<StoreProductCategory>?> GetCategoryName(string Name, long StoreId);

        public Task<IEnumerable<Products>?> GetProductName(string Name, long catagoryID);
        public Task<IEnumerable<Products>?> GetProductID(string Name, string ProductID);
        public Task<List<stockInventory>> GetProductByID(long id);
        public Task<IEnumerable<Products>?> GetProductExcel(long id);
        public Task<IEnumerable<stockInventory>?> GetAllProductData(long StoreID);

        public Task<IEnumerable<StoreProductCategory>?> GetCategoryDetail(long StoreID);
        public Task<List<Products>> GetProductDetails(long StoreID, long categoryID);
        public Task<IEnumerable<AddCart>?> POSCartList(long loggedinuser, long StoreId);

        public Task<IEnumerable<AddCart>?> GetAllCartDetails(long loggedinuser, long StoreId);
        public Task<IEnumerable<StoreProductCategory>?> DropDownListCategory(long StoreID);

        public Task<int>SaveProductDetail(long CategoryId, string ProductName, string ProductDesc, string ProductUniqueID, string ImagePath, decimal SellingPrice,long Quantity,long Threshold,long CustomerId);
        public Task<int> SaveProductInventory(stockInventory stockInventory);

        Task<long> RefundAmount(long communityId, long UserId, decimal Amount, string RefundNote, string Currency,long StoreId, long TransactionFrom);


        public Task<int> CheckoutPayment(Transactions transactions);

        public Task<bool> UpdateProductInventory(stockInventory stockInventory);
        public Task<int> UpdateTimeSlotes(StorefrontSchedule Schedule);
        public Task<int> SaveCategoryDetail(StoreProductCategory storeProductCategory);
        Task<long> DeleteStockAndProductAsync(long ProductId);
        public Task<IEnumerable<Transactions>?> Storewallet(long StoreID);

        public Task<int> AddPreOrderSlots(StorefrontSchedule storefrontSchedule);

        public Task<int> SaveIteminCart(AddCart addCart);
        public Task<IEnumerable<StorefrontSchedule>?> GetPreOrderList(long StoreID);

        public Task<IEnumerable<StorefrontSchedule>?> EditPreorder(long ID);

        public Task<IEnumerable<Order>?> BuyerProductList(long BuyerId, long CommunityId);
        #endregion

        Task<Order> GetOrder(long OrderId);
        Task<int> PayOrder(Order order);

        Task<int>CollectOrder(Order order);
        Task<List<CustomerDetails>> SendCollectEmail(long communityID);


        Task<IEnumerable<GetStore>?> Storefrontstore(long CommunityId, int PageSize, int PageNumber);
        Task<IEnumerable<dynamic>?> GetStoreCategory(long StoreId, long CommunityId, int PageSize, int PageNumber);
        Task<IEnumerable<dynamic>?> GetStoreProducts(long CategoryId,long? ProductId, int PageSize, int PageNumber);
        Task<IEnumerable<dynamic>?> AddToCart(long ProductId, long Quantity, decimal Amount, long Storeid, long loggedinuser);

        Task<IEnumerable<dynamic>?> CartList(long loggedinuser, long StoreId);
        Task<IEnumerable<dynamic>?> CheckOut(long Storeid, decimal TotalAmount, long noofitems, long OrderForId, long loggedinUser, string PromoCode);
        Task<IEnumerable<dynamic>?> UpdateCart(long ProductId, long Quantity, decimal Amount, long Storeid, long loggedinUser);
        Task<IEnumerable<Order>?> CollectedPendingOrder(long loggedinuser, long Collected, long? OrderId);

        Task<IEnumerable<dynamic>> ClearCart(long StoreId);
        Task<int> DeleteStore(long id);
        Task<clsStorefrontCustomerDetails> GetCustomerProductDetails(string MobileNo, long StoreId);

        Task<bool> UpdateOrderMarkAsCollected(long OrderId);
        public Task<CommunityTransportPass> GetQRCodePriceAsync(long id);
        public QR GetAttendanceQRCode(long id);
        Task<int> DeleteSalesItemsAsync(long id);
        public Task<int> SentReconEmail(long CommunityId,CashRegisterEmailRecon cashRegisterEmailRecon);
        //  Task<dynamic> StockEdit(long ProductId);
        Task<int> StockEdit(stockInventory stockInventory);
        Task<int> DeletePreOrderingAsync(long id);

        public Task<Order> CreateOrderQr1(Order order);
        Task<int> UpdateStore(CustomerStore item);
        Task<int> DleteMultiImageStore(long Id);
        Task<bool> StoreUploadImageAndFile6(long FundraiserId, string ImagePath);

    }
}
