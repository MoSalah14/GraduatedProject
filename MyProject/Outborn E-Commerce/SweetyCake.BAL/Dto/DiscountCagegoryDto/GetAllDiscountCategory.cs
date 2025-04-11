using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.DiscountCagegoryDto
{
    public class GetAllDiscountCategory
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid DiscountId { get; set; }
        public string CategoryNameArabic { get; set; }
        public string CategoryNameEndlish { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
