using Infrastructure.Services.PaymentWithStripeService;
using Infrastructure.Services.PaymentWithStripeService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using OutbornE_commerce.BAL.Dto.OrderDto;
using OutbornE_commerce.BAL.EmailServices;
using OutbornE_commerce.BAL.Repositories;
using OutbornE_commerce.BAL.Repositories.OrderRepo;
using OutbornE_commerce.BAL.Services.OrderService;
using OutbornE_commerce.Extensions;
using Stripe;

using Stripe.Checkout;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IOrderService orderService;
        private readonly IBagItemsRepo _BagItemsRepo;
        private readonly IPaymentWithStripeService paymentWithStripeService;
        private readonly IConfiguration configuration;
        public IHostEnvironment Environment { get; }
        public IWebHostEnvironment webHostEnvironment { get; }
        public IEmailSenderCustom _emailSender { get; }
        private readonly UserManager<User> _userManager;


        public OrderController(UserManager<User> userManager, IHostEnvironment _env, IWebHostEnvironment env, IEmailSenderCustom emailSender,
            IConfiguration configuration, IPaymentWithStripeService paymentWithStripeService,
            IOrderRepository orderRepository,
            IOrderService orderService,IBagItemsRepo bagItemsRepo)
        {
            this.orderRepository = orderRepository;
            this.orderService = orderService;
            _BagItemsRepo = bagItemsRepo;
            this.paymentWithStripeService = paymentWithStripeService;
            this.paymentWithStripeService = paymentWithStripeService;
            this.configuration = configuration;
            Environment = _env;
            webHostEnvironment = env;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        [HttpPost("filtered")]
        public async Task<IActionResult> GetFilteredOrders([FromBody] GetFillteringorders? filter, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var orders = await orderRepository.GetFilteredOrdersAsync(filter, pageNumber, pageSize);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            //string[] includes = new string[] { "OrderItems", "OrderItems.ProductSize" };
            string[] includes = new string[] { "OrderItems.ProductSize.Size", "OrderItems.ProductSize.ProductColor.Color", "OrderItems.ProductSize.ProductColor.ProductColorImages", "OrderItems.ProductSize.ProductColor.Product" };

            try
            {
                var items = string.IsNullOrEmpty(searchTerm)
                    ? await orderRepository.FindAllAsyncByPagination(null, pageNumber, pageSize, includes)
                    : await orderRepository.FindAllAsyncByPagination(
                        o => o.UserId.Contains(searchTerm) || o.OrderNumber.Contains(searchTerm),
                        pageNumber, pageSize, includes
                    );

                var data = items.Data.Adapt<List<OrderDto>>();

                return Ok(new PaginationResponse<List<OrderDto>>
                {
                    Data = data,
                    IsError = false,
                    Status = (int)StatusCodeEnum.Ok,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = items.TotalCount
                });
            }
            catch (Exception ex)
            {
                return Ok(new Response<Exception>
                {
                    Message = ex.Message,
                    IsError = true,
                    Status = (int)StatusCodeEnum.ServerError,
                });
            }
        }

        [HttpGet("GetAllUserOrders")]
        public async Task<IActionResult> GetAllUserOrders(int pageNumber = 1, int pageSize = 3, string? DateRange = "last30days")
        {
            string[] includes =
                new string[] { "OrderItems.ProductSize.Size",
                     "OrderItems.ProductSize.ProductColor.Color",
                     "OrderItems.ProductSize.ProductColor.ProductColorImages",
                     "OrderItems.ProductSize.ProductColor.Product" };

            var userID = User.GetUserIdFromToken();
            if (userID == null)
                return Unauthorized(new
                {
                    MessageEn = "Please Login First",
                    MessageAr = "برجاء تسجيل الدخول اولا"
                });

            try
            {
                DateTime? startDate = null;
                DateTime? endDate = DateTime.UtcNow.AddDays(1).AddTicks(-1);

                startDate = (DateRange?.ToLower()) switch
                {
                    "last15days" => startDate = endDate?.AddDays(-15),
                    "last30days" => startDate = endDate?.AddDays(-30),
                    "last6months" => startDate = endDate?.AddMonths(-6),
                    "thisyear" => startDate = new DateTime(endDate.Value.Year, 1, 1),
                    _ => null,
                };

                var items = await orderRepository.FindAllAsyncByPagination(
                    e => e.UserId == userID &&
                         (startDate == null || e.CreatedOn >= startDate) &&
                         (endDate == null || e.CreatedOn <= endDate),
                    pageNumber, pageSize, includes);
                var data = items.Data.Adapt<List<OrderDto>>();

                data = data.OrderByDescending(e => e.CreatedOn).ToList(); // Just For Now Correct Way Must Order in Db Use IQuryble

                return Ok(new PaginationResponse<List<OrderDto>>
                {
                    Data = data,
                    IsError = false,
                    Status = (int)StatusCodeEnum.Ok,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = items.TotalCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)StatusCodeEnum.ServerError,
                                  new PaginationResponse<Exception>
                                  {
                                      Data = ex,
                                      IsError = true,
                                      Status = (int)StatusCodeEnum.ServerError,
                                  });
            }
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(ms => ms.Value.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();

                return BadRequest(new Response<Guid>
                {
                    Message = "All fields are required",

                    IsError = true,
                    MessageAr = "كل الحقول مطلوبه",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            var userId = User.GetUserIdFromToken();
            var response = await orderService.CreateOrderAsync(model, userId, cancellationToken);

            if (response.IsError)
                return BadRequest(response);
            else
            {
                
                return Ok(response);

            }
        }

        [HttpPost("ReceiveWebhook")]
        public async Task<IActionResult> StripeWebhook(CancellationToken cancellationToken)
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"].ToString();

            try
            {
                var confirmOrder = new ConfirmOrderRequstDto();
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, configuration["Stripe:WebhookSecret"]);

                if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;

                    string userId = string.Empty;
                    if (session.Status == "complete")
                    {
                        session.Metadata.TryGetValue("paymentType", out var paymentType);
                        session.Metadata.TryGetValue("UserId", out userId);
                       
                    }
                    var user = await _userManager.FindByIdAsync(userId.ToString());

                    var templatePath = Path.Combine(webHostEnvironment.WebRootPath, "Templates", "ConfirmOrder.html");

                    if (!System.IO.File.Exists(templatePath))
                    {
                        return NotFound("Email template not found.");
                    }

                    var emailContent = System.IO.File.ReadAllText(templatePath);

                    if (!string.IsNullOrEmpty(emailContent))
                    {
                        emailContent = emailContent
                            .Replace("{{FullName}}", confirmOrder.FullName)
                            .Replace("{{OrderNumber}}", confirmOrder.OrderNumber)
                            .Replace("{{ShippingPrice}}", confirmOrder.ShippingPrice.ToString("F2"))
                            .Replace("{{TotalAmount}}", confirmOrder.TotalAmount.ToString("F2"))
                            .Replace("{{Address.Street}}", confirmOrder.Address.Street)
                            .Replace("{{Address.BuildingNumber}}", confirmOrder.Address.BuildingNumber)
                            .Replace("{{Address.AddressLine}}", confirmOrder.Address.AddressLine)
                            .Replace("{{Address.LandMark}}", confirmOrder.Address.LandMark);

                        // Replacing Order Items dynamically
                        string orderItemsHtml = "";
                        foreach (var item in confirmOrder.OrderItems)
                        {
                            orderItemsHtml += $@"
                            <div class='summary-item'>
                                <img src='{item.ImageUrl}' alt='{item.ProductNameAr}'>
                                <div class='summary-item-info'>
                                    <h3>{item.ProductNameAr}</h3>
                                    <p>{item.ColorNameAr} × {item.Quantity}</p>
                                </div>
                                <div class='summary-item-price'>AED {item.ItemPrice:F2}</div>
                            </div>";
                        }

                        emailContent = emailContent.Replace("{{OrderItems}}", orderItemsHtml);
                    }

                    await _emailSender.SendEmailAsync(user.Email, "Confirm Order", emailContent);


                }

                return Ok(new Response<string>
                {
                    Data = null,
                    IsError = false,
                    Message = $"Sucesses",
                    Status = (int)StatusCodeEnum.Ok,
                });
            }
            catch (StripeException ex)
            {
                return BadRequest(new { error = $"Stripe error: {ex.Message}" });
            }
        }

        [HttpGet("GetOrderByID/{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            string[] includes = new string[] { "OrderItems.ProductSize.Size", "OrderItems.ProductSize.ProductColor.Color", "OrderItems.ProductSize.ProductColor.ProductColorImages", "OrderItems.ProductSize.ProductColor.Product", "Delivery" };

            var order = await orderRepository.Find(i => i.Id == id, false, includes);
            if (order == null)
            {
                return BadRequest(new Response<OrderDto>
                {
                    Data = null,
                    Message = "Invalid Order Id",

                    MessageAr = "لم يتم العثور عن اوردر",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var data = order.Adapt<OrderDto>();
            if (order.PaymentStatus != PaymentStatus.UnPaid && order.PaymentMethod != DAL.Enums.PaymentMethod.Strip)
            {
                data.TrackingUrl = order.Delivery.TrackingUrl;
            }
            return Ok(new Response<OrderDto>
            {
                Data = data,
                IsError = false,

                Status = (int)StatusCodeEnum.Ok
            });
        }

        //[HttpPut("DeliverdOrder{id}")]
        //public async Task<IActionResult> StatusOrder(Guid id, CancellationToken cancellationToken)
        //{
        //    var order = await orderRepository.Find(i => i.Id == id, false, null);
        //    if (order == null)
        //    {
        //        return BadRequest(new Response<OrderDto>
        //        {
        //            Data = null,
        //            Message = "Invalid Order Id",
        //            IsError = true,
        //            Status = (int)StatusCodeEnum.NotFound
        //        });
        //    }

        //    order.OrderStatus = OrderStatus.Confirmed;
        //    orderRepository.Update(order);
        //    await orderRepository.SaveAsync(cancellationToken);
        //    return Ok(new Response<Guid>
        //    {
        //        Data = order.Id,
        //        Message = "تم وصول الاوردر",
        //        IsError = false,
        //        Status = (int)StatusCodeEnum.Ok
        //    });
        //}

        //[HttpPost("RefundPayment")]
        //public async Task<IActionResult> RefundPayment([FromBody] string sessionId)
        //{
        //    try
        //    {
        //        var refund = await paymentWithStripeService.RefundCheckoutSessionAsync(sessionId);

        //        return Ok(new Response<Refund>
        //        {
        //            Data = refund,
        //            IsError = false,
        //            Message = "Payment refunded successfully",
        //            Status = (int)StatusCodeEnum.Ok
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new Response<string>
        //        {
        //            IsError = true,
        //            Message = "Failed to refund payment: " + ex.Message,
        //            Status = (int)StatusCodeEnum.BadRequest
        //        });
        //    }
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var order = await orderRepository.Find(
                    c => c.Id == id,
                    false,
                    new string[] { "OrderItems", "Delivery" }
                );

                if (order == null)
                {
                    return Ok(new Response<string>
                    {
                        Data = null,
                        IsError = true,
                        Message = $"Order with Id: {id} doesn't exist in the database",
                        MessageAr = "لم يتم العثور على الطلب",
                        Status = (int)StatusCodeEnum.NotFound,
                    });
                }

                var hasOrderItems = order.OrderItems != null && order.OrderItems.Any();
                var hasDelivery = order.Delivery != null;

                if (hasOrderItems || hasDelivery)
                {
                    return Ok(new Response<Guid>
                    {
                        Data = id,
                        Message = "The Order cannot be deleted because it is connected to other data.",
                        MessageAr = "لا يمكن حذف الطلب لأنه مرتبط ببيانات أخرى.",
                        IsError = true,
                        Status = (int)StatusCodeEnum.BadRequest
                    });
                }

                order.IsDeleted = true;
                // order.OrderStatus = OrderStatus.CANCELLED;

                orderRepository.Update(order);

                await orderRepository.SaveAsync(cancellationToken);

                return Ok(new Response<string>
                {
                    Data = null,
                    IsError = false,
                    Message = $"Order with Id: {id} has been successfully soft deleted.",
                    MessageAr = "تم حذف الطلب بنجاح",
                    Status = (int)StatusCodeEnum.Ok,
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)StatusCodeEnum.ServerError, new Response<string>
                {
                    Data = null,
                    Message = "An error occurred while soft deleting the order",
                    MessageAr = "حدث خطأ أثناء حذف الطلب",
                    IsError = true,
                    Status = (int)StatusCodeEnum.ServerError
                });
            }
        }
    }
}