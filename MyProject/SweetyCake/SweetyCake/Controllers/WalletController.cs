using Infrastructure.Services.PaymentWithStripeService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.Wallet;
using OutbornE_commerce.BAL.Services.Wallet;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.Extensions;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IPaymentWithStripeService paymentWithStripe;
        private readonly WalletService walletService;

        public WalletController(IPaymentWithStripeService _paymentWithStripe, WalletService walletService)
        {
            this.paymentWithStripe = _paymentWithStripe;
            this.walletService = walletService;
        }

        [Authorize]
        [HttpPost("charge")]
        public async Task<IActionResult> ChargeWallet(decimal ChargeAmount, CancellationToken cancellationToken)
        {
            if (ChargeAmount <= 0)
                return BadRequest("Amount must be greater than zero.");

            var userId = User.GetUserIdFromToken();
            if (userId == null)
                return NotFound("User not found.");

            try
            {
                await walletService.CheckIfUserHaveWallet(userId, cancellationToken);

                var CreateSession = await paymentWithStripe.CreateWalletSession(userId, (long)ChargeAmount);

                return Ok(CreateSession);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize, HttpGet("GetUserWallet")]
        public async Task<IActionResult> GetUserWallet(CancellationToken cancellationToken)
        {
            var UserID = User.GetUserIdFromToken();
            if (UserID is null)
            {
                return Unauthorized(new Response<string>
                {
                    Data = null,
                    IsError = true,
                    Message = "Please Login First",
                    MessageAr = "برجاء تسجيل الدخول اولا",
                    Status = (int)StatusCodeEnum.Unauthorized
                });
            }

            var GetWallet = await walletService.GetUserWallet(UserID, cancellationToken);
            if (GetWallet is null)
            {
                return Ok(new Response<decimal>
                {
                    Data = 0,
                    IsError = false,
                    Message = "Successfully",
                    MessageAr = "تم بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
            else
            {
                return Ok(new Response<decimal>
                {
                    Data = GetWallet.Balance,
                    IsError = false,
                    Message = "Successfully",
                    MessageAr = "تم بنجاح",
                    Status = (int)StatusCodeEnum.Ok
                });
            }
        }
    }
}