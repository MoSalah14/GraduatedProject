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
        private readonly IConfiguration configuration;
        public IHostEnvironment Environment { get; }
        public IWebHostEnvironment webHostEnvironment { get; }
        public IEmailSenderCustom _emailSender { get; }
        private readonly UserManager<User> _userManager;


        public OrderController(UserManager<User> userManager, IHostEnvironment _env, IWebHostEnvironment env, IEmailSenderCustom emailSender,
            IConfiguration configuration,
            IOrderRepository orderRepository,
            IOrderService orderService, IBagItemsRepo bagItemsRepo)
        {
            this.orderRepository = orderRepository;
            this.orderService = orderService;
            _BagItemsRepo = bagItemsRepo;
            this.configuration = configuration;
            Environment = _env;
            webHostEnvironment = env;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            string[] includes = new string[] { "OrderItems.Product" };

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
        public async Task<IActionResult> GetAllUserOrders(int pageNumber = 1, int pageSize = 3, string? DateRange = "thisyear")
        {
            string[] includes =
                new string[] { "OrderItems.Product" };

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
                    pageNumber, pageSize, includes, e => e.OrderByDescending(e => e.CreatedOn));

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


        [HttpGet("GetOrderByID/{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            string[] includes = new string[] { "OrderItems.Product" };

            var order = await orderRepository.Find(i => i.Id == id, false, includes);
            if (order == null)
            {
                return BadRequest(new Response<OrderDto>
                {
                    Data = null,
                    Message = "Invalid Order Id",

                    MessageAr = "لم يتم العثور علي اوردر",
                    IsError = true,
                    Status = (int)StatusCodeEnum.NotFound
                });
            }

            var data = order.Adapt<OrderDto>();

            return Ok(new Response<OrderDto>
            {
                Data = data,
                IsError = false,

                Status = (int)StatusCodeEnum.Ok
            });
        }


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


        [HttpPost("ReceiveWebhook")]
        public async Task<IActionResult> StripeWebhook(CancellationToken cancellationToken)
        {
            string json;
            string stripeSignature;

            try
            {
                using (var reader = new StreamReader(HttpContext.Request.Body))
                {
                    json = await reader.ReadToEndAsync();
                }

                stripeSignature = Request.Headers["Stripe-Signature"];
                if (string.IsNullOrEmpty(stripeSignature))
                    return BadRequest(new { error = "Missing Stripe-Signature header." });


                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, configuration["Stripe:WebhookSecret"], throwOnApiVersionMismatch: false);

                if (stripeEvent == null)
                    return BadRequest(new { error = "Invalid Stripe event." });


                if (stripeEvent.Type != EventTypes.CheckoutSessionCompleted)
                    return BadRequest(new { error = "Unhandled event type." });


                var session = stripeEvent.Data.Object as Session;
                if (session == null)
                    return BadRequest(new { error = "Invalid session object." });


                if (session.Status != "complete")
                    return BadRequest(new { error = "Session is not complete." });


                if (!session.Metadata.TryGetValue("UserId", out var userId) || string.IsNullOrWhiteSpace(userId))
                    return BadRequest(new { error = "UserId is missing in session metadata." });


                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound(new { error = "User not found." });


                await _BagItemsRepo.ClearCartAsync(userId, cancellationToken);

                return Ok(new Response<string>
                {
                    Data = null,
                    IsError = false,
                    Message = "Success",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            catch (StripeException ex)
            {
                return BadRequest(new { error = $"Stripe error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"Internal error: {ex.Message}" });
            }
        }


    }
}