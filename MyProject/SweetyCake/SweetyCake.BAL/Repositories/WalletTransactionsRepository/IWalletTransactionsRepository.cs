using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Wallet;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.WalletTransactionsRepository
{
    public interface IWalletTransactionsRepository : IBaseRepository<WalletTransaction>
    {
       Task<PagainationModel<IEnumerable<GetWalletTransaction>>> GetFilteredWalletTransactions(
               FiltterationWalletDto? filter,
               int pageNumber,
               int pageSize,
               CancellationToken cancellationToken);  
    }
}