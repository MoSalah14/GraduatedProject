using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.discount
{
    public class UpdateDiscount
    { 
        public Guid Id { get; set; }

        [Range(0, 99, ErrorMessage = "The discount percentage must be between 0 and 100.")]
        public decimal Percentage { get; set; }
        public decimal Number { get; set; }


    }
}

