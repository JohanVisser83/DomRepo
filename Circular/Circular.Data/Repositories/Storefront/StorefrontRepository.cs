using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Framework.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using RepoDb;
using System.Data;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Circular.Data.Repositories.Storefront
{
    public class StorefrontRepository : DbRepository<SqlConnection>, IStorefrontRepository
    {
        private readonly IHelper _helper;
        IHttpContextAccessor _httpContextAccessor;
        public StorefrontRepository(string connectionString, IHelper helper, IHttpContextAccessor httpContextAccessor) : base(connectionString)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        }
        #region StoreFont Repository
        public async Task<int> SaveStoreDetail(CustomerStore customerStore)
        {

            var StoreData = await InsertAsync<CustomerStore, int>(customerStore);
            return StoreData;

        }

        public async Task<IEnumerable<CustomerStore>?> DropDownListStore(long CommunityId)
        {
            var StoreList = QueryAll<CustomerStore>().Where(e => e.IsActive == true && e.CommunityId == CommunityId).OrderByDescending(x => x.Id).ToList();
            return StoreList;
        }
        public async Task<IEnumerable<Order>?> OrderDetails(string Startdate, long CommunityId, long StoreId, string Enddate)
        {
            int Count = 0;


            var buyerList = "exec dbo.USP_GetSalesBuyerData" + " " + CommunityId + ",'" + Startdate + "','" + Enddate + "'," + Count + "," + StoreId + "";
            var result = await ExecuteQueryAsync<Order>(buyerList);
            return result;
        }
        public async Task<int> SaveCategoryDetail(StoreProductCategory storeProductCategory)
        {
            var CategoryList = await InsertAsync<StoreProductCategory, int>(storeProductCategory);
            return CategoryList;
        }
        public async Task<IEnumerable<CustomerStore>?> GetStoreName(long CommunityId, string Name)
        {
            var StoreData = QueryAll<CustomerStore>().Where(d => d.IsActive == true && d.CommunityId == CommunityId && d.StoreDisplayName == Name).ToList();
            return StoreData;
        }

        public async Task<IEnumerable<StoreProductCategory>?> GetCategoryName(string Name, long StoreId)
        {
            var Categorylist = QueryAll<StoreProductCategory>().Where(l => l.IsActive == true && l.CategoryName == Name && l.StoreId == StoreId
            ).ToList();
            return Categorylist;
        }
        public async Task<IEnumerable<StoreProductCategory>?> DropDownListCategory(long StoreID)
        {
            var DdlCategorylist = QueryAll<StoreProductCategory>().Where(l => l.IsActive == true && l.StoreId == StoreID).ToList();
            return DdlCategorylist;
        }

        public async Task<int> SaveProductDetail(long CategoryId, string ProductName, string ProductDesc, string ProductUniqueID, string ImagePath, decimal SellingPrice, long Quantity, long Threshold, long CustomerId)
        {



            var Param = new
            {
                CategoryId = CategoryId,
                ProductName = ProductName,
                ProductDesc = ProductDesc,
                ProductUniqueId = ProductUniqueID,
                ProductImage = ImagePath,
                SellingPrice = SellingPrice,
                Quantity = Quantity,
                Threshold = Threshold,
                LoggedInUserId = CustomerId
            };
            var result = (long)await ExecuteNonQueryAsync("[dbo].[Usp_Store_AddProduct]", Param, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        //public async Task<int> RefundAmount(Refund refund)
        //{
        //    var refundamount = await InsertAsync<Refund, int>(refund);
        //    return refundamount;
        //}

        public async Task<long> RefundAmount(long communityId, long UserId, decimal Amount, string RefundNote, string Currency, long StoreId, long TransactionFrom)
        {
            try
            {
                var result = await ExecuteNonQueryAsync("Exec [dbo].[Usp_PostRefund]' " + communityId + "'," + UserId + "," + Amount + ",'" + RefundNote + "','" + Currency + "'," + StoreId + "," + TransactionFrom);

                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<int> SaveProductInventory(stockInventory stockInventory)
        {
            var StockInventoryList = await InsertAsync<stockInventory, int>(stockInventory);

            return StockInventoryList;
        }



        public async Task<bool> UpdateProductInventory(stockInventory stockInventory)
        {
            try
            {
                string Query = "exec [dbo].[Usp_Store_UpdateStockDetail] " +
                               "@ProductId, @CategoryId, @Product, @Productcost, @Threshold, @ProductImage";

                var parameters = new
                {
                    stockInventory.ProductId,
                    stockInventory.CategoryId,
                    stockInventory.Product,
                    stockInventory.Productcost,
                    stockInventory.Threshold,
                    stockInventory.ProductImage
                };

                int status = await ExecuteNonQueryAsync(Query, parameters);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<int> UpdateTimeSlotes(StorefrontSchedule storefrontSchedule)
        {
            StorefrontSchedule entity = new StorefrontSchedule();
            entity.Id = storefrontSchedule.Id;
            entity.OpenTime = storefrontSchedule.OpenTime;
            entity.IsActive = true;
            entity.ClosedTime = storefrontSchedule.ClosedTime;
            entity.Title = storefrontSchedule.Title;
            entity.Days = storefrontSchedule.Days;
            var fields = Field.Parse<StorefrontSchedule>(o => new
            {
                o.Id,
                o.IsActive,
                o.ClosedTime,
                o.Title,
                o.Days,
                o.OpenTime,

            });
            int updatedrows = Update(entity: entity, fields: fields);
            return updatedrows;
        }
        public async Task<IEnumerable<Products>?> GetProductName(string Name, long Category)
        {
            var Categorylist = QueryAll<Products>().Where(l => l.IsActive == true && l.ProductName == Name && l.CategoryId == Category).ToList();
            return Categorylist;
        }

        public async Task<IEnumerable<Products>?> GetProductID(string Name, string ProductID)
        {
            var ProductList = QueryAll<Products>().Where(l => l.IsActive == true && l.ProductName == Name && l.ProductUniqueID == ProductID).ToList();
            return ProductList;
        }



        public async Task<List<Products>> GetProductDetails(long StoreID, long categoryID)
        {
            //try
            //{
            //    var result = QueryAll<Products>().Where(l => l.IsActive == true && l.CategoryId == categoryID).ToList();
            //    if (result != null)
            //    {
            //        foreach (var f in result)
            //        {
            //            f.Images = QueryAll<StoreProductImages>().Where(x => x.ProductId == f.Id && x.IsActive == true).ToList() ?? new List<StoreProductImages>();
            //        }
            //        return (List<Products>)result;
            //    }
            //    else
            //        return null;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
            try
            {
                var result = ExecuteQueryAsync<Products>("Usp_POSData " + StoreID + "," + categoryID + "").Result.ToList();
                if (result != null)
                {
                    foreach (var f in result)
                    {
                        f.Images = QueryAll<StoreProductImages>().Where(x => x.ProductId == f.Id && x.IsActive == true).ToList() ?? new List<StoreProductImages>();
                    }
                    return (List<Products>)result;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    



        public async Task<IEnumerable<stockInventory>?> GetAllProductData(long StoreID)
        {
            try
            {
                var result = ExecuteQuery<stockInventory>("exec [dbo].[Usp_GetStockInventory]" + StoreID);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        public async Task<List<stockInventory>> GetProductByID(long id)
        {
            try
            {
                var result = await ExecuteQueryAsync<stockInventory>("Exec [dbo].[Usp_GetStockInventoryEditData]" + " '" + id + "'");
                if (result != null)
                {
                    foreach (var f in result)
                    {
                        f.Images = QueryAll<StoreProductImages>().Where(x => x.ProductId == f.Id && x.IsActive == true).ToList() ?? new List<StoreProductImages>();
                    }
                    return (List<stockInventory>)result;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<IEnumerable<StoreProductCategory>?> GetCategoryDetail(long StoreID)
        {
            var CategoryData = QueryAll<StoreProductCategory>().Where(l => l.IsActive == true && l.StoreId == StoreID).ToList();
            return CategoryData;
        }
        public async Task<IEnumerable<Transactions>?> Storewallet(long StoreID)
        {
            var Storewallet = ExecuteQuery<Transactions>("exec [dbo].[Usp_StoreFront_Wallet]" + StoreID);
            return Storewallet;
        }
        public async Task<IEnumerable<Order>?> StorePreorderCustomer(string Mobile, long StoreId)
        {
            var CustomerPreOrder = ExecuteQuery<Order>("exec [dbo].[Usp_StoreCustomerPreAmount]'" + Mobile + "'," + StoreId + "");
            return CustomerPreOrder;
        }

        public async Task<int> CheckoutPayment(Transactions transactions)
        {
            var checkout = "Exec [dbo].[Usp_Storefront_Checkout] " + transactions.StoreId + "," + transactions.TotalAmount + ",'" + transactions.Quantity + "',0," + transactions.CustomerId + ",'cash'";
            var result = await ExecuteQueryAsync<dynamic>(checkout);
            if (result != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public async Task<IEnumerable<Products>?> GetProductExcel(long id)
        {
            var ExcelProductlist = ExecuteQuery<Products>("exec dbo.USP_GetStoreProductDetails " + id.ToString());
            return ExcelProductlist;
        }

        public async Task<long> DeleteStockAndProductAsync(long ProductId)
        {
            var deleteStock = ExecuteQueryAsync("Exec [dbo].[Usp_DeleteProductAndInventory] " + ProductId);
            return 1;
        }

        public async Task<int> AddPreOrderSlots(StorefrontSchedule storefrontSchedule)
        {
            var PreorderSlots = await InsertAsync<StorefrontSchedule, int>(storefrontSchedule);
            return PreorderSlots;

        }

        public async Task<IEnumerable<StorefrontSchedule>?> EditPreorder(long ID)
        {
            var ScheduleData = QueryAll<StorefrontSchedule>().Where(l => l.IsActive == true && l.Id == ID).ToList();
            return ScheduleData;
        }
        public async Task<IEnumerable<StorefrontSchedule>?> GetPreOrderList(long StoreID)
        {
            var Preorderlist = ExecuteQuery<StorefrontSchedule>("exec dbo.Usp_GetSchedulePreOrder " + StoreID.ToString()).OrderByDescending(i => i.StoreId).ToList();
            return Preorderlist;

        }

        public async Task<IEnumerable<AddCart>?> POSCartList(long loggedinuser, long StoreId)
        {
            var result = await ExecuteQueryAsync<AddCart>("Exec [dbo].[Usp_StoreFront_CartItemList]" + " '" + loggedinuser + "','" + StoreId + "';");
            return result;
        }
        public async Task<IEnumerable<AddCart>?> GetAllCartDetails(long loggedinuser, long StoreId)
        {
            var result = await ExecuteQueryAsync<AddCart>("Exec [dbo].[Usp_StoreFront_CartItemList]" + " '" + loggedinuser + "','" + StoreId + "';");
            return result;
        }
        public async Task<int> SaveIteminCart(AddCart addCart)
        {
            try
            {
                var cartlist = "Exec [dbo].[Usp_Storefront_AddToCart]" + "  '" + addCart.ProductId + "','" + addCart.Quantity + "'," + addCart.Amount + ",'" + addCart.StoreId + "','" + addCart.CustomerId + "';";
                var result = await ExecuteQueryAsync<dynamic>(cartlist);
                if (result != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }


        }
        public async Task<IEnumerable<Order>?> BuyerProductList(long BuyerId, long CommunityId)
        {
            var buyerList = "exec dbo.USP_OrderProductDetail" + " '" + BuyerId + "'";
            var result = await ExecuteQueryAsync<Order>(buyerList);
            return result;


        }

        public async Task<IEnumerable<Order>?> StoreCashRagister(long Community, long StoreId, string Startdate, string EndDate)
        {
            var AmountList = "exec dbo.USP_GetTotalDigitalSales" + " " + Community + ", " + StoreId + ",'" + Startdate + "','" + EndDate + "'";
            var result = await ExecuteQueryAsync<Order>(AmountList);
            return result;

        }
        #endregion
        public async Task<Order> GetOrder(long OrderId)
        {
            return QueryAsync<Order>(o => o.IsActive == true && o.Id == OrderId).Result.FirstOrDefault() ?? null;
        }
        public async Task<int> PayOrder(Order order)
        {
            order.ModifiedDate = DateTime.Now;
            order.IsPaid = true;
            var fields = Field.Parse<Order>(e => new
            {
                e.IsPaid,
                e.ModifiedBy,
                e.ModifiedDate
            });
            var updatedRows = Update<Order>(entity: order, fields: fields);
            return updatedRows;
        }


        public async Task<int> CollectOrder(Order order)
        {
            order.ModifiedDate = DateTime.Now;
            order.IsCollected = true;
            var fields = Field.Parse<Order>(e => new
            {
                e.IsPaid,
                e.ModifiedBy,
                e.ModifiedDate
            });
            var updatedRows = Update<Order>(entity: order, fields: fields);
            return updatedRows;
        }
        public async Task<List<CustomerDetails>> SendCollectEmail(long communityId)
        {
            var response = ExecuteQuery<CustomerDetails>("exec dbo.[USP_Storefront_SentCollectEmail] " + communityId).ToList();
            return response;
        }

        public async Task<IEnumerable<GetStore>?> Storefrontstore(long CommunityId, int PageSize, int PageNumber)
        {
            try
            {
				IEnumerable<GetStore>  result = await ExecuteQueryAsync<GetStore>("Exec [dbo].[Usp_StoreFront_GetCommunitystore]" + "  '" + CommunityId + "'");
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<dynamic>?> GetStoreCategory(long StoreId, long CommunityId, int PageSize, int PageNumber)
        {
            try
            {
                var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_Storefront_GetStoreCategories]" + " '" + StoreId + "','" + CommunityId + "','" + PageSize + "','" + PageNumber + "';");
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<dynamic>?> GetStoreProducts(long CategoryId, long? productId, int PageSize, int PageNumber)
        {
            try
            {
                var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Storefront_GetProducts]" + " '" + CategoryId + "','" + productId + "','" + PageSize + "','" + PageNumber + "';");
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<dynamic>?> AddToCart(long ProductId, long Quantity, decimal Amount, long Storeid, long loggedinuser)
        {
            try
            {
                var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Storefront_AddToCart]" + "  '" + ProductId + "','" + Quantity + "','" + Amount + "','" + Storeid + "','" + loggedinuser + "';");
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<dynamic>?> CartList(long loggedinuser, long StoreId)
        {
            try
            {
                var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_StoreFront_CartItemList]" + " '" + loggedinuser + "','" + StoreId + "';");
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<dynamic>?> CheckOut(long Storeid, decimal TotalAmount, long noofitems, long OrderForId, long loggedinUser, string promoCode)
        {
            try
            {
                var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Storefront_Checkout]" + "  '" + Storeid + "','" + TotalAmount + "','" + noofitems + "','" + OrderForId + "','" + loggedinUser + "','" + promoCode + "';");
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<IEnumerable<dynamic>?> UpdateCart(long ProductId, long Quantity, decimal Amount, long Storeid, long loggedinUser)
        {
            try
            {
                var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_StoreFront_UpdateCart]  '" + ProductId + "','" + Quantity + "','" + Amount + "','" + Storeid + "','" + loggedinUser + "'");
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Order>?> CollectedPendingOrder(long loggedinuser, long Collected, long? OrderId)
        {
            try
            {
                List<Order> orders = new List<Order>();
                orders = (await ExecuteQueryAsync<Order>("Exec [dbo].[Usp_StoreFront_PendingCollectedOrder]  '" + loggedinuser + "','" + Collected + "','" + OrderId + "'")).ToList();
                foreach (var order in orders)
                {
                    try
                    {
                        string QRCodePath = Directory.GetCurrentDirectory() + "/Uploads/QRs/Orders/";
                        var browsePath = _httpContextAccessor?.HttpContext.Request.Scheme + "://" + _httpContextAccessor?.HttpContext.Request.Host + "/Uploads/QRs/Orders/";
                        string filename = _helper.EncryptUsingSHA1Hashing(order.Id.ToString()) + ".png";
                        order.QRCode = _helper.GetQRCode(order.Id.ToString(), filename, ref QRCodePath);
                    }
                    catch (Exception ex)
                    {
                        order.QRCode = "";
                    }
                    order.OrderedForName = QueryAll<CustomerDetails>().Where(c => c.IsActive == true && c.CustomerId == order.OrderdForId)?.FirstOrDefault()?.FirstName + " " + QueryAll<CustomerDetails>().Where(c => c.IsActive == true && c.CustomerId == order.OrderdForId)?.FirstOrDefault()?.LastName;
                    order.orderDetails = QueryAll<OrderDetails>().Where(l => l.IsActive == true && l.OrderId == order.Id).ToList() ?? new List<OrderDetails>();
                    if (order.orderDetails.Count > 0)
                    {
                        foreach (var item in order.orderDetails)
                        {
                            item.ProductImage = QueryAll<Products>().Where(p => p.IsActive == true && p.Id == item.ProductId).FirstOrDefault()?.ProductImage;
                            item.ProductName = QueryAll<Products>().Where(p => p.IsActive == true && p.Id == item.ProductId).FirstOrDefault()?.ProductName;
                        }
                    }
                }

                return orders;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<IEnumerable<dynamic>> ClearCart(long StoreId)
        {
            try
            {
                return await ExecuteQueryAsync<dynamic>("Exec [dbo].[usp_clearcart] " + StoreId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> DeleteStore(long id)
        {
            CustomerStore delete = new CustomerStore();
            delete.Id = id;
            delete.IsActive = false;
            var fields = Field.Parse<CustomerStore>(x => new
            {
                x.IsActive


            });
            var updaterow = await UpdateAsync<CustomerStore>(entity: delete, fields: fields);
            return updaterow;
        }

        public async Task<clsStorefrontCustomerDetails> GetCustomerProductDetails(string MobileNo, long StoreId)
        {
            decimal? Totalamount = 0;
            int? totalitem = 0;
            clsStorefrontCustomerDetails clsStorefrontCustomerDetails = new clsStorefrontCustomerDetails();
            var tuple = QueryMultiple<Order, OrderDetails, CustomerDetails, Customers, Products>(o => o.IsActive == true, od => od.IsActive == true, cd => cd.IsActive == true, c => c.IsActive == true, p => p.IsActive == true);
            if (tuple != null)
            {
                long CustomerId = Convert.ToInt64(tuple.Item4?.Where(x => x.Mobile.Replace("+", "") == MobileNo).FirstOrDefault()?.Id);
                clsStorefrontCustomerDetails.Name = tuple.Item3?.Where(x => x.CustomerId == CustomerId).FirstOrDefault()?.FirstName ?? "" + ' ' + tuple.Item3?.Where(x => x.CustomerId == CustomerId).FirstOrDefault()?.LastName ?? "";
                clsStorefrontCustomerDetails.MobileNo = tuple.Item4?.Where(x => x.Id == CustomerId).FirstOrDefault()?.Mobile ?? "";
                clsStorefrontCustomerDetails.OrderNo = Convert.ToInt64(tuple.Item1?.Where(x => x.StoreId == StoreId && x.CustomerId == CustomerId).FirstOrDefault().Id);
                foreach (var item in tuple.Item1?.Where(x => x.StoreId == StoreId && x.CustomerId == CustomerId))
                {
                    CustomerProductDetails customerProductDetails = new CustomerProductDetails();
                    customerProductDetails.Amount = Convert.ToDecimal(tuple.Item2?.Where(x => x.OrderId == item.Id).FirstOrDefault()?.TotalAmount);
                    customerProductDetails.Quantity = Convert.ToInt32(tuple.Item2?.Where(x => x.OrderId == item.Id).FirstOrDefault()?.Quantity);
                    customerProductDetails.ProductId = Convert.ToInt64(tuple.Item2?.Where(x => x.OrderId == item.Id).FirstOrDefault()?.ProductId);
                    customerProductDetails.ProductName = tuple.Item5?.Where(x => x.Id == customerProductDetails.ProductId).FirstOrDefault()?.ProductName;
                    Totalamount += customerProductDetails.Amount;
                    totalitem += customerProductDetails.Quantity;
                    clsStorefrontCustomerDetails.lstcustomerProductDetails.Add(customerProductDetails);
                }
                clsStorefrontCustomerDetails.TotalPrice = Totalamount ?? 0;
                clsStorefrontCustomerDetails.TotalItem = totalitem ?? 0;
            }
            else
                return null;
            return clsStorefrontCustomerDetails;
        }

        public async Task<bool> UpdateOrderMarkAsCollected(long OrderId)
        {
            Order update = new Order();
            update.Id = OrderId;
            update.IsCollected = true;
            var fields = Field.Parse<Order>(x => new
            {
                x.IsCollected
            });
            var updaterow = await UpdateAsync<Order>(entity: update, fields: fields);
            if (updaterow > 0)
                return true;
            else
                return false;
        }

        public async Task<CommunityTransportPass> GetQRCodePriceAsync(long id)
        {
            var results = QueryAll<CommunityTransportPass>().Where(e => e.Id == id && e.IsActive == true).ToList().FirstOrDefault();
            string QRCodePath = Directory.GetCurrentDirectory() + "/Uploads/QRs/Vehicle/";
            var browsePath = _httpContextAccessor?.HttpContext.Request.Scheme + "://" + _httpContextAccessor?.HttpContext.Request.Host + "/Uploads/QRs/Vehicle/";
            string filename = _helper.EncryptUsingSHA1Hashing(results.Price.ToString()) + ".png";
            results.QRCode.QRCode = _helper.GetQRCode(results.Price.ToString(), filename, ref QRCodePath);
            results.QRCode.QRPath = browsePath + filename;
            if (results != null)
                return results;
            else
                return null;
        }
        public QR GetAttendanceQRCode(long id)
        {
            QR attendanceqr = new QR();
            string QRCodePath = Directory.GetCurrentDirectory() + "/Uploads/QRs/Vehicle/";
            var browsePath = _httpContextAccessor?.HttpContext.Request.Scheme + "://" + _httpContextAccessor?.HttpContext.Request.Host + "/Uploads/QRs/Vehicle/";
            string filename = _helper.EncryptUsingSHA1Hashing(id.ToString()) + ".png";
            attendanceqr.QRCode = _helper.GetQRCode(id.ToString(), filename, ref QRCodePath);
            attendanceqr.QRPath = browsePath + filename;
            return attendanceqr;
        }

        public async Task<int> DeleteSalesItemsAsync(long id)
        {
            OrderDetails order = new OrderDetails() { Id = id, ModifiedBy = 101, ModifiedDate = DateTime.Now, IsActive = false };
            var fields = Field.Parse<OrderDetails>(x => new
            {
                x.IsActive,
                x.ModifiedBy,
                x.ModifiedDate
            });
            var updatedrow = await UpdateAsync<OrderDetails>(entity: order, fields: fields);
            return updatedrow;
        }



        public async Task<IEnumerable<Order>?> SentReconEmail(long Community, long StoreId, string Startdate, string EndDate)
        {
            var AmountList = "exec dbo.USP_GetTotalDigitalSales" + " " + Community + ", " + StoreId + ",'" + Startdate + "','" + EndDate + "'";
            var result = await ExecuteQueryAsync<Order>(AmountList);
            return result;

        }

        //public async Task<dynamic> StockEdit(long ProductId)
        //{
        //    return await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_StockEditProduct] '" + ProductId + "'");
        //}
        public async Task<int> StockEdit(stockInventory stockInventory)
        {
            try
            {
                stockInventory.FillDefaultValues();
                var key = await InsertAsync<stockInventory, int>(stockInventory);




                return key;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<Order> CreateOrderQr1(Order orders)
        {
            try
            {
                orders.FillDefaultValues();
                var key = await InsertAsync<Order, int>(orders);
                Order order = QueryAll<Order>().Where(e => e.Id == orders.Id && e.IsActive == true).ToList().FirstOrDefault();

                string QRCodePath = Directory.GetCurrentDirectory() + "/Uploads/QRs/OrderId/";
                var browsePath = _httpContextAccessor?.HttpContext.Request.Scheme + "://" + _httpContextAccessor?.HttpContext.Request.Host + "/Uploads/QRs/OrderId/";
                string filename = _helper.EncryptUsingSHA1Hashing(order.Id.ToString()) + ".png";
                order.QRCode1.QRCode = _helper.GetQRCode(order.Id.ToString(), filename, ref QRCodePath);
                order.QRCode1.QRPath = browsePath + filename;
                if (order != null)
                    return order;
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<int> DeletePreOrderingAsync(long id)
        {
            StorefrontSchedule delete = new StorefrontSchedule();
            delete.Id = id;
            delete.IsActive = false;
            var fields = Field.Parse<StorefrontSchedule>(x => new
            {
                x.IsActive


            });
            var updaterow = await UpdateAsync<StorefrontSchedule>(entity: delete, fields: fields);
            return updaterow;
        }
        public async Task<int> UpdateStore(CustomerStore item)
        {
            try
            {
                CustomerStore store = new CustomerStore();
                store.Id = item.Id;
                store.StoreDisplayName = item.StoreDisplayName;
                store.DisplayImage = item.DisplayImage;


                store.UpdateModifiedByAndDateTime();
                var fields = Field.Parse<CustomerStore>(x => new
                {

                    x.StoreDisplayName,
                    x.DisplayImage,

                    x.ModifiedBy,
                    x.ModifiedDate
                });
                return await UpdateAsync<CustomerStore>(entity: store, fields: fields);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<int> DleteMultiImageStore(long Id)
        {
            StoreProductImages delete = new StoreProductImages();
            delete.Id = Id;
            delete.IsActive = false;
            var fields = Field.Parse<StoreProductImages>(x => new
            {
                x.IsActive

            });
            var updaterow = Update<StoreProductImages>(entity: delete, fields: fields);
            return updaterow;
        }

        public async Task<bool> StoreUploadImageAndFile6(long ProductId, string ImagePath)
        {
            String Query = "exec [dbo].[USP_Storefront_AddStoreProductImage] '" + ProductId + "','" + ImagePath + "'";
            int status = await ExecuteNonQueryAsync(Query);
            return true;
        }

    }
}
