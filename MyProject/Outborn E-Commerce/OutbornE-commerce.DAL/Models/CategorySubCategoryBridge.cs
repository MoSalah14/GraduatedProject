using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class CategorySubCategoryBridge:BaseEntity
    {
     

        public Guid CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public Guid SubCategoryId { get; set; }
        public virtual SubCategory? SubCategory { get; set; }
      
    }
}
