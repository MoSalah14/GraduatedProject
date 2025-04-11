using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Cart
{
    public class GetCartResponseDto
    {
        public List<GetUserCart> CartItems { get; set; }
        public decimal TotalCartPrice { get; set; }
        public decimal TotalProductWeight { get; set; }

    }
}