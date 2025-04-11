using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class WalletTransaction : BaseEntity
    {
        public Guid UserWalletId { get; set; }

        public decimal Amount { get; set; }

        public string TransactionType { get; set; } // Debit Or Credit

        public string? StripeSessionId { get; set; }

        // Navigation property to UserWallet
        public virtual Wallets UserWallet { get; set; }
    }
}