using OutbornE_commerce.BAL.Repositories.BaseRepositories;
using OutbornE_commerce.BAL.Repositories.WalletTransactionsRepository;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.Wallet
{
    public class WalletRepository : BaseRepository<Wallets>, IWalletRepository
    {
        public WalletRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task UpdateAmount(string userId, decimal Amount, CancellationToken cancellationToken)
        {
            var GetUserWallet = await Find(e => e.UserId == userId, true);

            if (GetUserWallet != null)
            {
                GetUserWallet.Balance += Amount;
                GetUserWallet.UpdatedOn = DateTime.Now;
                GetUserWallet.UpdatedBy = "User";
            }

            await SaveAsync(cancellationToken);
        }
    }
}