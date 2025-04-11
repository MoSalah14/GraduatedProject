using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Repositories.Wallet;
using OutbornE_commerce.BAL.Repositories.WalletTransactionsRepository;
using OutbornE_commerce.DAL.Enums;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.BAL.Services.Wallet
{
    public class WalletService
    {
        private readonly IWalletRepository walletRepository;
        private readonly IWalletTransactionsRepository walletTransactions;

        public WalletService(IWalletRepository walletRepository, IWalletTransactionsRepository walletTransactions)
        {
            this.walletRepository = walletRepository;
            this.walletTransactions = walletTransactions;
        }

        public async Task UpdateAmount(string userId, decimal Amount, CancellationToken cancellationToken)
        {
            await walletRepository.UpdateAmount(userId, Amount, cancellationToken);
        }

        public async Task RecordWalletTransactionAsync(string UserId, decimal amount, string sessionId, CancellationToken cancellationToken)
        {
            if (UserId == null) throw new ArgumentNullException(nameof(UserId));
            if (amount <= 0) throw new ArgumentException("Transaction amount must be positive", nameof(amount));

            var GetWalletUserId = await walletRepository.Find(e => e.UserId == UserId);
            if (GetWalletUserId == null)
                throw new ArgumentNullException(nameof(GetWalletUserId));

            var transaction = new WalletTransaction
            {
                UserWalletId = GetWalletUserId.Id,
                Amount = amount,
                TransactionType = TransactionType.Recharge.ToString(),
                StripeSessionId = sessionId,
                IsDeleted = false
            };

            try
            {
                var x = await walletTransactions.Create(transaction);
                await walletTransactions.SaveAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in webhook: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        public async Task CheckIfUserHaveWallet(string UserId, CancellationToken cancellationToken)
        {
            // If User Not Have Wallet Create One

            var CheckWallet = await walletRepository.Find(e => e.UserId == UserId);
            if (CheckWallet == null)
            {
                var CreateWallet = new Wallets
                {
                    UserId = UserId,
                    Balance = 0,
                    CreatedBy = "User",
                    CreatedOn = DateTime.UtcNow,
                };

                await walletRepository.Create(CreateWallet);
                await walletRepository.SaveAsync(cancellationToken);
            }
        }

        public async Task PayWithWalletAsync(string userId, decimal amount, CancellationToken cancellationToken)
        {
            // Validate if payment amount is positive
            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero.", nameof(amount));

            var userWallet = await walletRepository.Find(e => e.UserId == userId, true);

            if (userWallet is null)
                throw new InvalidOperationException("User wallet not found.");

            // Validate if the user has enough balance for the payment
            if (userWallet.Balance < amount)
                throw new InvalidOperationException("Insufficient balance in wallet for this payment.");

            userWallet.Balance -= amount;
            await walletRepository.SaveAsync(cancellationToken);

            //  Add a transaction record
            var transaction = new WalletTransaction
            {
                UserWalletId = userWallet.Id,
                Amount = amount,
                CreatedOn = DateTime.UtcNow,
                TransactionType = TransactionType.Payment.ToString(),
            };
            await walletTransactions.Create(transaction);
            await walletTransactions.SaveAsync(cancellationToken);
        }

        public async Task<Wallets> GetUserWallet(string userId, CancellationToken cancellationToken)
             => await walletRepository.Find(e => e.UserId == userId);
    }
}