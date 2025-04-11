using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.BAL.Dto.Wallet;
using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.WalletTransactionsRepository
{
    public class WalletTransactionsRepository : BaseRepository<WalletTransaction>, IWalletTransactionsRepository
    {
        public WalletTransactionsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PagainationModel<IEnumerable<GetWalletTransaction>>> GetFilteredWalletTransactions(
      FiltterationWalletDto? filter,
      int pageNumber,
      int pageSize,
      CancellationToken cancellationToken)
        {
            Expression<Func<WalletTransaction, bool>>? criteria = null;

            if (filter != null)
            {
                criteria = t =>
                    (string.IsNullOrEmpty(filter.UserName) || t.UserWallet.User.UserName.Contains(filter.UserName)) &&
                    (!filter.StartDate.HasValue || t.CreatedOn >= filter.StartDate.Value) &&
                    (!filter.EndDate.HasValue || t.CreatedOn <= filter.EndDate.Value);
            }

            string[] includes = { "UserWallet.User" };

            var paginatedResult = await FindAllAsyncByPagination(criteria, pageNumber, pageSize, includes);

            var transactions = paginatedResult.Data.Select(t => new GetWalletTransaction
            {
                Name = t.UserWallet.User.FullName,
                Date = t.CreatedOn,
                Amount = t.Amount,
                PaymentMethod = t.TransactionType
            });

            return new PagainationModel<IEnumerable<GetWalletTransaction>>()
            {
                Data = transactions,
                PageNumber = paginatedResult.PageNumber,
                PageSize = paginatedResult.PageSize,
                TotalCount = paginatedResult.TotalCount
            };
        }

    }
}