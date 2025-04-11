using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class SubCategory:BaseEntity
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
      
        public virtual ICollection<SubCategoryType> SubCategoryTypes { get; set; }
        public virtual ICollection<CategorySubCategoryBridge> CategorySubCategories { get; set; }


    }
}
