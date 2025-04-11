using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OutbornE_commerce.DAL.Models
{
    public class ProductSize : BaseEntity
    {
        public Guid ProductColorId { get; set; }

        public virtual ProductColor? ProductColor { get; set; }

        public Guid SizeId { get; set; }

        public Size? Size { get; set; }

        public string SKU_Size { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal ProductWeight { get; set; }

        public decimal DiscountedPrice { get; set; } = 0;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public virtual ICollection<ProductSizeDiscount>? ProductSizeDiscounts { get; set; }
    }
}