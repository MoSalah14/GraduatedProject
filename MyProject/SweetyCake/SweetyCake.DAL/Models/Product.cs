using OutbornE_commerce.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutbornE_commerce.DAL.Models
{
    public class Product : BaseEntity
    {
        [MaxLength(DBRules.ProductNameLenght)]
        public string NameEn { get; set; }

        [MaxLength(DBRules.ProductNameLenght)]
        public string NameAr { get; set; }

        [MaxLength(DBRules.ParagraphLenght)]
        public string AboutEn { get; set; }

        [MaxLength(DBRules.ParagraphLenght)]
        public string AboutAr { get; set; }
        [MaxLength(DBRules.CaptionLenght)]

        public string MaterialEn { get; set; }

        [MaxLength(DBRules.CaptionLenght)]
        public string MaterialAr { get; set; }
        public int QuantityInStock { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public bool IsPreOrder { get; set; } = false;

        public bool IsActive { get; set; } = true;
        public string? MainImageUrl { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
        public virtual ICollection<BagItem> BagItem { get; set; } = new List<BagItem>();

        public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();
        public virtual ICollection<ProductImage> ProductImage { get; set; } = new List<ProductImage>();
        public virtual ICollection<OrderItem> OrderItems { get; set; }

    }
}