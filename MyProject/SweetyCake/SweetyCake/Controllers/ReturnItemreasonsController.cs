using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto.ReturnItemreasonDto;
using OutbornE_commerce.BAL.Repositories.ReturnedImagesRep;
using OutbornE_commerce.BAL.Repositories.ReturnedItems;
using OutbornE_commerce.BAL.Services.DeliveryService;
using OutbornE_commerce.BAL.Services.OrderService;
using OutbornE_commerce.Extensions;
using OutbornE_commerce.FilesManager;
using Stripe.Climate;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnItemReasonsController : ControllerBase
    {
        private readonly IOrderService _OrderService;
        private readonly DeliveryService _DeliveryService;

        public ReturnItemReasonsController(IOrderService orderService, DeliveryService deliveryService)
        {
            _OrderService = orderService;
            _DeliveryService = deliveryService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReturnOrder([FromForm] ReturnedOrdersDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid || model.ReturnItems == null || !model.ReturnItems.Any())
            {
                return BadRequest(new Response<Guid>
                {
                    Message = "Invalid data",
                    IsError = true,
                    MessageAr = "بيانات غير صالحة",
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }

            try
            {
                var userId = User.GetUserIdFromToken();

                var AddReturnOrder = await _OrderService.CreateReturnOrder(userId, model, cancellationToken);

                if (AddReturnOrder is null)
                {
                    return BadRequest(new Response<string>
                    {
                        Message = "An error occurred while connecting",
                        IsError = true,
                        Status = (int)StatusCodeEnum.BadRequest
                    });
                }

                var IsSuccess = await _DeliveryService.CreateReturnedDeliveryOrderAsync(AddReturnOrder);
                if (IsSuccess)
                {
                    return Ok(new Response<string>
                    {
                        IsError = false,
                        Status = (int)StatusCodeEnum.Ok,
                        Message = "Return Order Successfully",
                    });
                }
                else
                {
                    return BadRequest(new Response<string>
                    {
                        Message = "Ops,An error occurred",
                        IsError = true,
                        Status = (int)StatusCodeEnum.BadRequest
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<string>
                {
                    Message = "An error occurred while connecting",
                    IsError = true,
                    MessageAr = ex.InnerException?.Message ?? ex.Message,
                    Status = (int)StatusCodeEnum.BadRequest
                });
            }
        }
    }
}