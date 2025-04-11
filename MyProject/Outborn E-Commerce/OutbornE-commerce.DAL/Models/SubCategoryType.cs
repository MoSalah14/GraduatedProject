using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class SubCategoryType:BaseEntity
    {
       
        public Guid SubCategoryId { get; set; }
        public virtual SubCategory? SubCategory { get; set; }

        public Guid TypeId { get; set; }
        public virtual TypeEntity? type { get; set; } 
      
  
    }
}
