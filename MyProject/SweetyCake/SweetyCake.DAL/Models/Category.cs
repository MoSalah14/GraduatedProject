using System.ComponentModel.DataAnnotations;

namespace OutbornE_commerce.DAL.Models
{
    public class Category : BaseEntity
    {

        [MaxLength(DBRules.ProductNameLenght)]
        public string NameEn { get; set; }

        [MaxLength(DBRules.ProductNameLenght)]
        public string NameAr { get; set; }
      
        public virtual ICollection<Product> CategorySubCategories { get; set; }
    }
}