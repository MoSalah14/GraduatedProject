using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.discount
{
    public class GetAllDiscount
    {
        public Guid Id { get; set; }
        public decimal Percentage { get; set; }
        public decimal Number { get; set; }

    }
}
