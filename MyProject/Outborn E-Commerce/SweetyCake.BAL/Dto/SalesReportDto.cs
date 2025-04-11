using OutbornE_commerce.DAL.Enums;
using System.Text.Json.Serialization;

namespace OutbornE_commerce.BAL.Dto
{
    public class SalesReportDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Period { get; set; }
        public int? BrandNumber { get; set; }
        public string? BrandName { get; set; }
        public string? ProductSKU { get; set; }
        public string? ProductName { get; set; }

        public string? Status { get; set; }

        public DateTime? DateAdded { get; set; }
        public decimal? RetailPrice { get; set; }
        public int? PeriodStartInventory { get; set; }
        public int? PeriodEndInventory { get; set; }
        public int? PeriodSalesCount { get; set; }
        public decimal? PeriodSalesAmount { get; set; }
        public int? TotalSalesCount { get; set; }
        public decimal? TotalSalesAmount { get; set; }
        public int? PeriodReturnsCount { get; set; }
        public decimal? PeriodReturnsAmount { get; set; }
    }
}