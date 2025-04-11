using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Profile
{
    public class UserDisplayDto
    {
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        //public string AccountType { get; set; }
        public decimal WalletBalance { get; set; }
    }
}
