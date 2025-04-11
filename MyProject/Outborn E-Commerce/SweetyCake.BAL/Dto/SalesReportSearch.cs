using OutbornE_commerce.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace OutbornE_commerce.BAL.Dto
{
    public class SalesReportSearch
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? ProductName { get; set; }
        public string? SKU { get; set; }
        public string? BrandName { get; set; }
        public bool? Status { get; set; }
    }
}