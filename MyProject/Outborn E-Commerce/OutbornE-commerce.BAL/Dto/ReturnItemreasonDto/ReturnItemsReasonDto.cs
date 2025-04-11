using Microsoft.AspNetCore.Http;
using OutbornE_commerce.BAL.Dto.ProductSizes;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.ReturnItemreasonDto
{
    public class ReturnItemsReasonDto
    {
        public Guid? Id { get; set; }
        public int Quantity { get; set; }
        public string Comment { get; set; }
        public Guid? ReturnOrderId { get; set; }
        public Guid ProductSizeId { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}