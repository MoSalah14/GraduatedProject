namespace OutbornE_commerce.DAL.Models
{
    public class Category : BaseEntity
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
      
        public virtual ICollection<Product> CategorySubCategories { get; set; }
    }
}