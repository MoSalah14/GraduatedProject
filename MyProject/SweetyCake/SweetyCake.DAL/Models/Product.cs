using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutbornE_commerce.DAL.Models
{
    public class Product : BaseEntity
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }

        public string AboutEn { get; set; }
        public string AboutAr { get; set; }
        public string MaterialEn { get; set; }
        public string MaterialAr { get; set; }
        public int QuantityInStock { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }

        public bool IsPreOrder { get; set; } = false;

        public bool IsActive { get; set; } = true;
        public string MainImageUrl { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();

        public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();
        public virtual ICollection<ProductImage> ProductImage { get; set; } = new List<ProductImage>();
    }
}