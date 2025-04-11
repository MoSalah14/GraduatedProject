using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutbornE_commerce.DAL.Models;

namespace OutbornE_commerce.DAL.Models
{
    public class Wallets : BaseEntity
    {
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<WalletTransaction> Transactions { get; set; }
    }
}