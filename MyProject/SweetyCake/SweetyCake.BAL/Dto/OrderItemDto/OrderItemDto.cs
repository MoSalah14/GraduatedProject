using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.OrderItemDto
{
    public class OrderItemDto
    {
        public Guid? Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductSizeId { get; set; }

        public string ProductNameEn { get; set; }
        public string ProductNameAr { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }
        public int NumberOfReturn { get; set; }
        public string ColorNameEn { get; set; }
        public string ColorNameAr { get; set; }
        public string Size { get; set; }
        public string ImageUrl { get; set; }
        public string ProductCode { get; set; }

    }
}