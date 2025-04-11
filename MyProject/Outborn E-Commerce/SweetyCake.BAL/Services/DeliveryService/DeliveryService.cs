using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OutbornE_commerce.BAL.Dto.Delivery;
using OutbornE_commerce.BAL.Dto.Delivery.DeliveryOrders;
using OutbornE_commerce.BAL.Dto.Delivery_Order_Response;
using OutbornE_commerce.BAL.Repositories.ProductSizes;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;
using System.Net.Http.Json;

namespace OutbornE_commerce.BAL.Services.DeliveryService
{
    public class DeliveryService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext dbContext;
        private readonly IProductSizeRepository _ProductSizeRepository;
        private const string ApiKey = "95ebf457-b514-4e2b-bb57-b73ca9c054c4";

        public DeliveryService(HttpClient httpClient, ApplicationDbContext _dbContext, IProductSizeRepository productSizeRepository)
        {
            _httpClient = httpClient;
            dbContext = _dbContext;
            _ProductSizeRepository = productSizeRepository;
            _httpClient.BaseAddress = new Uri("https://staging.integrator.swftbox.com/api/direct-integration/");
            _httpClient.DefaultRequestHeaders.Add("x-api-key", ApiKey);
        }

        public async Task CreateDeliveryOrderAsync(DeliveryObject deliveryObject, Guid OrderID, bool IsVisa, bool IsOrderExpress = false)
        {
            var deliveryOrder = await PrepareDeliveryOrderAsync(deliveryObject, IsVisa, IsOrderExpress);
            var response = await _httpClient.PostAsJsonAsync("orders", deliveryOrder);
            response.EnsureSuccessStatusCode();

            var JsonResponse = await response.Content.ReadAsStringAsync();
            var ConvertToObject = JsonConvert.DeserializeObject<DeliveryOrderDataResponse>(JsonResponse).Data.FirstOrDefault();

            var CreatedeliveryOrder = new DeliveryOrder
            {
                OrderId = OrderID,
                TrackingNumber = ConvertToObject.TrackingNumber,
                shippingLabelUrl = ConvertToObject.ShippingLabelUrl,
                TrackingUrl = ConvertToObject.TrackingUrl,
                Status = ConvertToObject.Status,
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin",
            };
            await dbContext.AddAsync(CreatedeliveryOrder);
        }

        public async Task<bool> CreateReturnedDeliveryOrderAsync(ReturnedDeliveryObject deliveryObject)
        {
            try
            {
                var ReturneddeliveryOrder = await PrepareReturnedDeliveryOrderAsync(deliveryObject);
                var response = await _httpClient.PostAsJsonAsync("orders", ReturneddeliveryOrder);
                response.EnsureSuccessStatusCode();

                var JsonResponse = await response.Content.ReadAsStringAsync();
                var ConvertToObject = JsonConvert.DeserializeObject<DeliveryOrderDataResponse>(JsonResponse).Data.FirstOrDefault();

                var CreatedeliveryOrder = new DeliveryOrder
                {
                    ReturnedOrdersID = deliveryObject.ReturnedOrderID,
                    TrackingNumber = ConvertToObject.TrackingNumber,
                    shippingLabelUrl = ConvertToObject.ShippingLabelUrl,
                    TrackingUrl = ConvertToObject.TrackingUrl,
                    Status = ConvertToObject.Status,
                    CreatedOn = DateTime.Now,
                    CreatedBy = "Admin",
                    IsReverse = true,
                };
                await dbContext.AddAsync(CreatedeliveryOrder);
                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Helper Method

        private async Task<DeliveryOrderRequest> PrepareDeliveryOrderAsync(DeliveryObject deliveryObject, bool IsPaymentVisa, bool IsOrderExpress)
        {
            var GetUserInfo = await dbContext.Users.FirstOrDefaultAsync(e => e.Id == deliveryObject.UserID);

            var getAddress = await dbContext.Addresses.Include(e => e.Country).Include(e => e.City).FirstOrDefaultAsync(d => d.Id == deliveryObject.AddressID);

            if (GetUserInfo is null || getAddress is null)
                throw new Exception("User or address not found.");

            var CustomerInfo = new CustomerInfo
            {
                Name = GetUserInfo.FullName,
                Phone = GetUserInfo.PhoneNumber,
                AddressLine1 = getAddress.AddressLine,
                Building = getAddress.BuildingNumber,
                City = getAddress?.City?.NameEn ?? "Test ",
                Country = getAddress!.Country.NameEn,
                CountryCode = getAddress.Country.CountryCode,
                LandMark = getAddress.LandMark,
                Street = getAddress.Street
            };
            var ProductsItemsForDelivery = new List<ProductItem>();

            var PrepareDeliveryObject = new DeliveryOrderRequest
            {
                Reference = deliveryObject.OrderNumber,
                PaymentAmount = !IsPaymentVisa ? deliveryObject.TotalAmount : 0,
                CustomerInfo = CustomerInfo,
                Items = deliveryObject.ProductItems,
                profileName = IsOrderExpress ? "sameday" : "nextday"
            };

            return PrepareDeliveryObject;
        }

        private async Task<ReturnedOrderRequest> PrepareReturnedDeliveryOrderAsync(ReturnedDeliveryObject deliveryObject) // OverLoad
        {
            var GetUserInfo = await dbContext.Users.FirstOrDefaultAsync(e => e.Id == deliveryObject.UserID);

            var getAddress = await dbContext.Addresses.Include(e => e.Country).Include(e => e.City).FirstOrDefaultAsync(d => d.Id == deliveryObject.AddressID);

            if (GetUserInfo is null || getAddress is null)
                throw new Exception("User or address not found.");

            var CustomerInfo = new CustomerInfo
            {
                Name = GetUserInfo.FullName,
                Phone = GetUserInfo.PhoneNumber,
                AddressLine1 = getAddress.AddressLine,
                Building = getAddress.BuildingNumber,
                City = getAddress?.City?.NameEn ?? "N/A",
                Country = getAddress!.Country.NameEn,
                CountryCode = getAddress.Country.CountryCode,
                LandMark = getAddress.LandMark,
                Street = getAddress.Street
            };

            List<ProductItem> ProductsItemsForDelivery = await GetProductItemDetails(deliveryObject);

            var PrepareDeliveryObject = new ReturnedOrderRequest
            {
                Reference = deliveryObject.OrderNumber,
                CustomerInfo = CustomerInfo,
                Items = ProductsItemsForDelivery,
                IsReverse = true,
                RetailerLocationIdentifier = 2110
            };

            return PrepareDeliveryObject;
        }

        private async Task<List<ProductItem>> GetProductItemDetails(ReturnedDeliveryObject deliveryObject)
        {
            var ProductsItemsForDelivery = new List<ProductItem>();

            foreach (var item in deliveryObject.ProductItems)
            {
                var GetProductData = await _ProductSizeRepository.Find(e => e.Id == item.ProductSizeId, false, new string[] { "ProductColor.Product" });

                var PItem = new ProductItem
                {
                    Name = GetProductData.ProductColor.Product.NameEn,
                    skuNumber = GetProductData.ProductColor.Product.SKU,
                    sku = GetProductData.ProductColor.Product.NameEn,
                    Weight = (double)GetProductData.ProductWeight,
                    Quantity = item.Quantity,
                };
                ProductsItemsForDelivery.Add(PItem);
            }

            return ProductsItemsForDelivery;
        }

        #endregion Helper Method

        public async Task<bool> CancelShippingOrder(Guid OrderID)
        {
            var GetOrder = await dbContext.DeliveryOrder.Include(i => i.Order).FirstOrDefaultAsync(e => e.Order.Id == OrderID);
            if (GetOrder is null) return false;

            var trackingNumber = GetOrder.TrackingNumber;

            //var url = $"https://staging.integrator.swftbox.com/api/direct-integration/orders/{trackingNumber}/cancel";

            var response = await _httpClient.PostAsJsonAsync($"orders/{trackingNumber}/cancel", new { });
            response.EnsureSuccessStatusCode();
            var JsonResponse = await response.Content.ReadAsStringAsync();

            var GetOrderStatus = JsonConvert.DeserializeObject<DeliveryOrderDataResponse>(JsonResponse).Data.FirstOrDefault();
            if (GetOrderStatus != null && GetOrderStatus.Status == OrderStatus.CANCELLED.ToString())
            {
                GetOrder.Status = OrderStatus.CANCELLED.ToString();
                GetOrder.UpdatedBy = "admin";
                GetOrder.UpdatedOn = DateTime.Now;
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
                return false;
        }
    }
}