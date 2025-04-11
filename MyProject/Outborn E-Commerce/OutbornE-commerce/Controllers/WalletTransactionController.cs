using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Wallet;
using OutbornE_commerce.BAL.Repositories.WalletTransactionsRepository;
using OutbornE_commerce.BAL.Services.Wallet;
using OutbornE_commerce.Extensions;

namespace OutbornE_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletTransactionController : ControllerBase
    {
        public IWalletTransactionsRepository WalletTransactionsRepository { get; }

        public WalletTransactionController(IWalletTransactionsRepository walletTransactionsRepository)
        {
            WalletTransactionsRepository = walletTransactionsRepository;
        }

        [HttpPost("GetFilteredTransactions")]
        public async Task<IActionResult> GetFilteredWalletTransactions(CancellationToken cancellationToken,[FromBody] FiltterationWalletDto? filter, int pageNumber = 1, int pageSize = 10)
        {
           
          
            var transactions = await WalletTransactionsRepository.GetFilteredWalletTransactions(filter,pageNumber,pageSize, cancellationToken);

            if (transactions.Data == null || !transactions.Data.Any())
            {
                return Ok(new Response<List<GetWalletTransaction>>
                {
                    Data = new List<GetWalletTransaction>(),
                    IsError = false,
                    Message = "No transactions found",
                    MessageAr = "لا توجد معاملات",
                    Status = (int)StatusCodeEnum.Ok
                });
            }

            return Ok(new Response<PagainationModel<IEnumerable<GetWalletTransaction>>>()
            {
                Data = transactions,
                IsError = false,
                Message = "Transactions fetched successfully",
                MessageAr = "تم جلب المعاملات بنجاح",
                Status = (int)StatusCodeEnum.Ok
            });
        }

    }
}
