namespace OutbornE_commerce.DAL.Models
{
    public class Category : BaseEntity
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
      
        public virtual ICollection<DiscountCategory>? DiscountCategories { get; set; }
        public virtual ICollection<CategorySubCategoryBridge> CategorySubCategories { get; set; }

        //    public Guid? SuperCategoryID { get; set; }

        //    public Category SuperCategory { get; set; }
        //    public ICollection<Category> Categories { get; set; }
        //    public virtual ICollection<DiscountCategory>? DiscountCategories { get; set; }
        //    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }
}