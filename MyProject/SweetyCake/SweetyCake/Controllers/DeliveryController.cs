using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OutbornE_commerce.BAL.Dto.Delivery;
using OutbornE_commerce.BAL.Dto.Delivery_Order_Response;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.OrderRepo;
using OutbornE_commerce.BAL.Repositories.ProductSizes;
using OutbornE_commerce.BAL.Services.DeliveryService;
using Serilog;
using System.Text;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IProductSizeRepository productSizeRepository;
        private readonly IBaseRepository<DeliveryOrder> baseRepository;
        private readonly DeliveryService deliveryService;

        public DeliveryController(IOrderRepository orderRepository, IProductSizeRepository productSizeRepository,
            IBaseRepository<DeliveryOrder> baseRepository, DeliveryService deliveryService)
        {
            this.orderRepository = orderRepository;
            this.productSizeRepository = productSizeRepository;
            this.baseRepository = baseRepository;
            this.deliveryService = deliveryService;
        }

        [HttpPost("StatusUpdate")]
        public async Task<IActionResult> UpdateOrderStatus(CancellationToken cancellationToken)
        {
            try
            {
                // Read request body
                using var reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8);
                var requestBody = await reader.ReadToEndAsync();

                Log.Information("Webhook received with raw data: {RequestBody}", requestBody);

                var deliveryOrderData = JsonConvert.DeserializeObject<DeliveryWebhookResponse>(requestBody);

                if (deliveryOrderData is null || deliveryOrderData.Order is null)
                {
                    Log.Warning("Received null or invalid deliveryOrderData.");
                    return BadRequest("Invalid data.");
                }

                var GetSpecifecOrder = new DeliveryOrder();

                //Converts bool? to bool and defaults to false if null.
                if (deliveryOrderData.Order != null && (deliveryOrderData.Order.IsReverse ?? false))
                {
                    GetSpecifecOrder = await baseRepository.Find(e =>
                           e.returnedOrders.OrderReturnNumber == deliveryOrderData.Order.Reference, true, ["returnedOrders"]);
                    if (GetSpecifecOrder is null)
                    {
                        Log.Warning("Order not found for reference: {Reference}", deliveryOrderData.Order!.Reference);
                        return BadRequest("Order not found.");
                    }
                }
                else
                {
                    GetSpecifecOrder = await baseRepository.Find(
                        e => e.Order.OrderNumber == deliveryOrderData.Order!.Reference, true, ["Order"]);

                    if (GetSpecifecOrder is null)
                    {
                        Log.Warning("Order not found for reference: {Reference}", deliveryOrderData.Order!.Reference);
                        return BadRequest("Order not found.");
                    }
                }

                GetSpecifecOrder.shippingLabelUrl = deliveryOrderData.Order!.ShippingLabelUrl!;
                GetSpecifecOrder.TrackingUrl = deliveryOrderData.Order.TrackingUrl!;
                GetSpecifecOrder.Comment = deliveryOrderData.Comment;
                GetSpecifecOrder.eventTrigger = deliveryOrderData.EventTrigger;
                GetSpecifecOrder.Status = deliveryOrderData.Order.Status!;
                GetSpecifecOrder.UpdatedOn = DateTime.UtcNow;
                baseRepository.Update(GetSpecifecOrder);
                await baseRepository.SaveAsync(cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing webhook.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> UpdateOrderStatus(Guid OrderID, CancellationToken cancellationToken)
        {
            try
            {
                var checkOrder = await deliveryService.CancelShippingOrder(OrderID);

                if (!checkOrder)
                {
                    return BadRequest(new Response<Guid>
                    {
                        Data = OrderID,
                        IsError = true,
                        Message = "Order Not Found",
                        MessageAr = "خطأ فى رقم الطلب"
                    });
                }

                string[] includes = { "OrderItems" };

                var order = await orderRepository.Find(x => x.Id == OrderID, false, includes);

                if (order == null)
                {
                    return NotFound(new Response<Guid>
                    {
                        Data = OrderID,
                        IsError = true,
                        Message = "Order Not Found",
                        MessageAr = "الطلب غير موجود"
                    });
                }

                await productSizeRepository.UpdateProductQuantities(order, cancellationToken);

                return Ok(new Response<Guid>
                {
                    Data = OrderID,
                    IsError = false,
                    Message = "Canceled Successfully",
                    MessageAr = "تم مسح الطلب بنجاح"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response<Guid>
                {
                    Data = OrderID,
                    IsError = true,
                    Message = "An error occurred while processing the request.",
                    MessageAr = "حدث خطأ أثناء معالجة الطلب"
                });
            }
        }
    }
}