using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class TypeEntity :BaseEntity
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
        public virtual ICollection<SubCategoryType> SubCategoryTypes { get; set; }

    }
}
