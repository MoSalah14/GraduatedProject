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

        public string SizeAndFitEn { get; set; }

        public string SizeAndFitAr { get; set; }

        public string DeliveryAndReturnEn { get; set; }

        public string DeliveryAndReturnAr { get; set; }

        public string SKU { get; set; }

        public Guid BrandId { get; set; }

        public Brand? Brand { get; set; }

        public ProductLabelEnum Label { get; set; }

        public int NumberOfReturnDays { get; set; }

        public bool IsPeopleAlseBought { get; set; }

        public bool IsPreOrder { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public string ProductCode { get; set; }

        public Guid? ProductTypeId { get; set; }

        public PreOrderDetails? PreOrderDetails { get; set; }

        public virtual ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();

        public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();

        public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();

        public virtual TypeEntity? ProductType { get; set; }
    }
}