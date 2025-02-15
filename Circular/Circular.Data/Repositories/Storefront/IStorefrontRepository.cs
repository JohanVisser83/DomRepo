using Circular.Core.DTOs;
using Circular.Core.Entity;
using Google.Api.Gax;
using System.Reflection;

namespace Circular.Data.Repositories.Storefront
{
    public interface IStorefrontRepository
    {
        #region
        Task<int> SaveStoreDetail(CustomerStore customerStore);

        Task<IEnumerable<CustomerStore>?> DropDownListStore(long CommunityId);

        Task<IEnumerable<CustomerStore>?> GetStoreName(long CommunityId, string Name);
        Task<IEnumerable<Products>?> GetProductName(string Name,long CategoryID);

        Task<IEnumerable<Products>?> GetProductID(string Name, string ProductID);
       
        public Task<List<stockInventory>> GetProductByID(long id);
        Task<IEnumerable<Order>?> OrderDetails(string Startdate, long CommunityId, long StoreId, string Enddate);
        Task<IEnumerable<Order>?> StoreCashRagister(long Community, long StoreId, string Startdate, string EndDate);
        Task<IEnumerable<Products>?> GetProductExcel(long id);
        Task<IEnumerable<stockInventory>?> GetAllProductData(long StoreID);
        Task<IEnumerable<StoreProductCategory>> GetCategoryName(string Name, long StoreId);
        Task<IEnumerable<StoreProductCategory>?> DropDownListCategory(long StoreID);
        Task<int> SaveCategoryDetail(StoreProductCategory storeProductCategory);
        Task<IEnumerable<StoreProductCategory>?> GetCategoryDetail(long StoreID);
        Task<IEnumerable<StorefrontSchedule>?> GetPreOrderList(long StoreID);
        Task<IEnumerable<StorefrontSchedule>?> EditPreorder(long ID);
        Task<IEnumerable<AddCart>?> POSCartList(long loggedinuser,long StoreId);
        Task<int> UpdateTimeSlotes(StorefrontSchedule storefrontSchedule);
        Task<IEnumerable<AddCart>?> GetAllCartDetails(long loggedinuser, long StoreId);
        public Task<List<Products>> GetProductDetails(long StoreID, long categoryID);
        Task<IEnumerable<Transactions>?> Storewallet(long wallet);
        Task<IEnumerable<Order>?> BuyerProductList(long BuyerId, long CommunityId);
        Task<int> SaveProductDetail(long CategoryId, string ProductName, string ProductDesc, string ProductUniqueID, string ImagePath, decimal SellingPrice, long Quantity, long Threshold, long CustomerId);
        Task<long> RefundAmount(long communityId, long UserId, decimal Amount, string RefundNote, string Currency,long StoreId, long TransactionFrom);
        Task<int> CheckoutPayment(Transactions transactions);
        Task<int> SaveProductInventory(stockInventory stockInventory);
        Task<IEnumerable<Order>?> StorePreorderCustomer(string Mobile,long StoreId);
        Task<bool> UpdateProductInventory(stockInventory stockInventory);
        Task<int> AddPreOrderSlots(StorefrontSchedule storefrontSchedule);
        Task<int> SaveIteminCart(AddCart addCart);
        Task<long> DeleteStockAndProductAsync(long ProductId);
        #endregion

        Task<Order> GetOrder(long OrderId);
        Task<int> PayOrder(Order order);

        Task<int> CollectOrder(Order order);
        Task<List<CustomerDetails>> SendCollectEmail(long communityID);
        Task<IEnumerable<GetStore>?> Storefrontstore(long CommunityId, int PageSize, int PageNumber);

        Task<IEnumerable<dynamic>?> GetStoreCategory(long StoreId, long CommunityId, int PageSize, int PageNumber);
        
        Task<IEnumerable<dynamic>?> GetStoreProducts(long CategoryId,long? ProductId, int PageSize, int PageNumber);
        Task<IEnumerable<dynamic>?> AddToCart(long ProductId, long Quantity, decimal Amount, long Storeid, long loggedinuser);
        Task<IEnumerable<dynamic>?> CartList(long loggedinuser, long StoreId);
        Task<IEnumerable<dynamic>?> CheckOut(long Storeid, decimal TotalAmount, long noofitems, long OrderForId, long loggedinUser, string PromoCode);
        Task<IEnumerable<dynamic>?> UpdateCart(long ProductId, long Quantity, decimal Amount, long Storeid, long loggedinUser);
        Task<IEnumerable<Order>?> CollectedPendingOrder(long loggedinuser, long Collected,long? OrderId);
        Task<IEnumerable<dynamic>> ClearCart(long StoreId);
        Task<int> DeleteStore(long id);
        Task<clsStorefrontCustomerDetails> GetCustomerProductDetails(string MobileNo, long StoreId);
        Task<bool> UpdateOrderMarkAsCollected(long OrderId);
        public Task<CommunityTransportPass> GetQRCodePriceAsync(long id);
        public QR GetAttendanceQRCode(long id);
        Task<int> DeleteSalesItemsAsync(long id);
        Task<IEnumerable<Order>?> SentReconEmail(long communityId, long storeId, string startdate, string endDate);
        //  Task<dynamic> StockEdit(long ProductId);
        Task<int> StockEdit(stockInventory stockInventory);
        public Task<Order> CreateOrderQr1(Order order);
        Task<int> DeletePreOrderingAsync(long id);
        Task<int> UpdateStore(CustomerStore item);
        Task<int> DleteMultiImageStore(long Id);
        Task<bool> StoreUploadImageAndFile6(long FundraiserId, string ImagePath);
    }
}
