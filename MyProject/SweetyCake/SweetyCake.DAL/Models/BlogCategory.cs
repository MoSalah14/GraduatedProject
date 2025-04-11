using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.DAL.Models
{
    public class BlogCategory : BaseEntity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public ICollection<Blogs>? Blogs { get; set; }
    }
}
